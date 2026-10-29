using System.Data.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Vettingo.AnalyticsService.Application.UnitOfWork;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;
using Vettingo.AnalyticsService.Persistence.Registration;
using Vettingo.AnalyticsService.Persistence.Repository;
using AnalyticsUnitOfWork = Vettingo.AnalyticsService.Persistence.UnitOfWork.UnitOfWork;

namespace Vettingo.AnalyticsService.UnitTests.Persistence;

public sealed class ProcessedEventAndUnitOfWorkTests
{
    [Fact]
    public void ProcessedEvent_Should_Use_The_Supplied_EventId_As_Its_Only_Key()
    {
        using var context = new AnalyticsDbContext(new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options);
        var model = context.Model.FindEntityType(typeof(ProcessedEvent))!;

        model.GetTableName().Should().Be("ProcessedEvents");
        model.FindPrimaryKey()!.Properties.Should().ContainSingle().Which.Name.Should().Be(nameof(ProcessedEvent.EventId));
        model.FindProperty(nameof(ProcessedEvent.EventId))!.ValueGenerated.Should().Be(ValueGenerated.Never);
    }

    [Fact]
    public async Task Repositories_Should_Use_The_UnitOfWork_Context_And_Query_By_EventId()
    {
        var services = new ServiceCollection();
        services.SaveDb(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unused;Username=unused;Password=unused"
        }).Build());
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var registered = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        registered.Should().BeSameAs(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        var registeredContext = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
        var registeredMarker = new ProcessedEvent(Guid.CreateVersion7());
        await registered.ProcessedEventRepository.AddAsync(registeredMarker);
        registeredContext.Entry(registeredMarker).State.Should().Be(EntityState.Added);

        await using var context = new AnalyticsDbContext(new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await using var unitOfWork = new AnalyticsUnitOfWork(context);
        var marker = new ProcessedEvent(Guid.CreateVersion7());
        var statistic = new CompanyStatistic(Guid.NewGuid());

        await unitOfWork.ProcessedEventRepository.AddAsync(marker);
        await unitOfWork.AnalyticsRepository.AddCompanyStatisticAsync(statistic);
        context.Entry(marker).State.Should().Be(EntityState.Added);
        context.Entry(statistic).State.Should().Be(EntityState.Added);
        (await unitOfWork.ProcessedEventRepository.ExistsAsync(marker.EventId)).Should().BeFalse();

        await unitOfWork.SaveChangesAsync();
        context.ChangeTracker.Clear();

        (await unitOfWork.ProcessedEventRepository.ExistsAsync(marker.EventId)).Should().BeTrue();
        (await unitOfWork.ProcessedEventRepository.ExistsAsync(Guid.NewGuid())).Should().BeFalse();
        (await context.CompanyStatistics.SingleAsync()).CompanyId.Should().Be(statistic.CompanyId);
    }

    [Fact]
    public async Task Commit_Should_Use_The_Active_Transaction_And_Dispose_It()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        using var cancellation = new CancellationTokenSource();
        context.Database.BeginTransactionAsync(cancellation.Token).Returns(transaction);

        await using (var unitOfWork = new AnalyticsUnitOfWork(context))
        {
            await unitOfWork.BeginTransactionAsync(cancellation.Token);
            await unitOfWork.CommitTransactionAsync(cancellation.Token);
        }

        await transaction.Received(1).CommitAsync(cancellation.Token);
        await transaction.Received(1).DisposeAsync();
        await transaction.DidNotReceiveWithAnyArgs().RollbackAsync(default);
    }

    [Fact]
    public async Task Rollback_Should_Clear_Tracked_Event_And_Dispose_Transaction()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        context.Add(new ProcessedEvent(Guid.CreateVersion7()));
        await using var unitOfWork = new AnalyticsUnitOfWork(context);

        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.RollbackTransactionAsync();

        context.ChangeTracker.Entries().Should().BeEmpty();
        await transaction.Received(1).RollbackAsync(CancellationToken.None);
        await transaction.Received(1).DisposeAsync();
    }

    [Fact]
    public async Task Failed_Commit_Should_Allow_Rollback()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        transaction.CommitAsync(CancellationToken.None).Returns(Task.FromException(new InvalidOperationException("Commit failed")));
        await using var unitOfWork = new AnalyticsUnitOfWork(context);
        await unitOfWork.BeginTransactionAsync();

        Func<Task> commit = () => unitOfWork.CommitTransactionAsync();
        await commit.Should().ThrowAsync<InvalidOperationException>();
        await unitOfWork.RollbackTransactionAsync();

        await transaction.Received(1).RollbackAsync(CancellationToken.None);
        await transaction.Received(1).DisposeAsync();
    }

    [Fact]
    public async Task Disposing_An_Active_UnitOfWork_Should_Roll_Back_And_Keep_Context_Usable()
    {
        await using var context = new TestDbContext();
        var transaction = Substitute.For<IDbContextTransaction>();
        context.Database.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        await using (var unitOfWork = new AnalyticsUnitOfWork(context))
        {
            await unitOfWork.BeginTransactionAsync();
        }

        await transaction.Received(1).RollbackAsync(CancellationToken.None);
        await transaction.Received(1).DisposeAsync();
        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task Increment_Should_Translate_Both_Counters_To_PostgreSql_Without_Opening_A_Connection()
    {
        var queryPlans = new List<string>();
        var options = new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseNpgsql("Host=localhost;Database=unused")
            .LogTo(queryPlans.Add, [CoreEventId.QueryExecutionPlanned])
            .AddInterceptors(new StopBeforeConnectionInterceptor()).Options;
        await using var context = new AnalyticsDbContext(options);
        Func<Task> action = () => new AnalyticsRepository(context)
            .IncrementCompanyJobPostingCountAsync(Guid.NewGuid());

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("SQL translation checked; connection skipped.");
        string.Join(Environment.NewLine, queryPlans).Should().Contain("TotalJobPostingCount")
            .And.Contain("ActiveJobPostingCount").And.Contain("UpdatedAt");
    }

    private sealed class TestDbContext : AnalyticsDbContext
    {
        public TestDbContext() : base(new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options)
        {
            Database = Substitute.For<DatabaseFacade>(this);
        }

        public override DatabaseFacade Database { get; }
    }

    private sealed class StopBeforeConnectionInterceptor : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("SQL translation checked; connection skipped.");
    }
}
