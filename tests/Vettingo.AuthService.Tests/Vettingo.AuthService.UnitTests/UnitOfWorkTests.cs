using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.UnitOfWork;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Persistence.Registration;
using AuthDbContext = Vettingo.AuthService.Persistence.DbContext.IdentityDbContext;
using AuthUnitOfWork = Vettingo.AuthService.Persistence.UnitOfWork.UnitOfWork;

namespace Vettingo.AuthService.UnitTests;

public sealed class UnitOfWorkTests
{
    [Fact]
    public async Task Identity_Repository_And_UnitOfWork_Should_Use_The_Same_Scoped_Context()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.SaveDb(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unused;Username=unused;Password=unused"
        }).Build());
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var store = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();

        store.GetType().GetProperty("Context")!.GetValue(store).Should().BeSameAs(context);
        unitOfWork.Should().BeSameAs(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        unitOfWork.CompanyRepository.Should().BeSameAs(scope.ServiceProvider.GetRequiredService<ICompanyRepository>());
        Company company = new();
        company.RegisterCompany("Vettingo");
        await unitOfWork.CompanyRepository.AddCompanyAsync(company);
        context.Entry(company).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task Save_And_Commit_Should_Use_The_Context_And_Dispose_Transaction()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        using var cancellation = new CancellationTokenSource();
        context.Database.BeginTransactionAsync(cancellation.Token).Returns(transaction);
        await using (var unitOfWork = new AuthUnitOfWork(context, Substitute.For<ICompanyRepository>()))
        {
            await unitOfWork.BeginTransactionAsync(cancellation.Token);
            (await unitOfWork.SaveChangesAsync(cancellation.Token)).Should().Be(3);
            await unitOfWork.CommitTransactionAsync(cancellation.Token);
        }

        context.SaveCount.Should().Be(1);
        context.SaveToken.Should().Be(cancellation.Token);
        await transaction.Received(1).CommitAsync(cancellation.Token);
        await transaction.Received(1).DisposeAsync();
        await transaction.DidNotReceiveWithAnyArgs().RollbackAsync(default);
    }

    [Fact]
    public async Task Rollback_Should_Dispose_Transaction_And_Clear_Rolled_Back_Tracked_Entities()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        Company company = new();
        company.RegisterCompany("Vettingo");
        context.Add(company);
        await using var unitOfWork = new AuthUnitOfWork(context, Substitute.For<ICompanyRepository>());

        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.RollbackTransactionAsync();

        context.ChangeTracker.Entries().Should().BeEmpty();
        await transaction.Received(1).RollbackAsync(CancellationToken.None);
        await transaction.Received(1).DisposeAsync();
        await transaction.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact]
    public async Task Failed_Commit_Should_Leave_Transaction_Available_For_Rollback()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        transaction.CommitAsync(CancellationToken.None).Returns(Task.FromException(new InvalidOperationException("Commit failed")));
        await using var unitOfWork = new AuthUnitOfWork(context, Substitute.For<ICompanyRepository>());
        await unitOfWork.BeginTransactionAsync();

        Func<Task> commit = () => unitOfWork.CommitTransactionAsync();
        await commit.Should().ThrowAsync<InvalidOperationException>();
        await unitOfWork.RollbackTransactionAsync();

        await transaction.Received(1).RollbackAsync(CancellationToken.None);
        await transaction.Received(1).DisposeAsync();
    }

    [Fact]
    public async Task Disposing_UnitOfWork_With_Active_Transaction_Should_Roll_Back_Without_Disposing_Context()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        await using (var unitOfWork = new AuthUnitOfWork(context, Substitute.For<ICompanyRepository>()))
        {
            await unitOfWork.BeginTransactionAsync();
        }

        await transaction.Received(1).RollbackAsync(CancellationToken.None);
        await transaction.Received(1).DisposeAsync();
        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task Begin_Should_Reject_A_Second_Active_Transaction()
    {
        await using var context = new TestDbContext();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(Substitute.For<IDbContextTransaction>());
        await using var unitOfWork = new AuthUnitOfWork(context, Substitute.For<ICompanyRepository>());
        await unitOfWork.BeginTransactionAsync();

        Func<Task> begin = () => unitOfWork.BeginTransactionAsync();
        await begin.Should().ThrowAsync<InvalidOperationException>();
        await context.Database.Received(1).BeginTransactionAsync(CancellationToken.None);
    }

    private sealed class TestDbContext : AuthDbContext
    {
        public TestDbContext() : base(new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options)
        {
            Database = Substitute.For<DatabaseFacade>(this);
        }

        public override DatabaseFacade Database { get; }
        public int SaveCount { get; private set; }
        public CancellationToken SaveToken { get; private set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            SaveToken = cancellationToken;
            return Task.FromResult(3);
        }
    }
}
