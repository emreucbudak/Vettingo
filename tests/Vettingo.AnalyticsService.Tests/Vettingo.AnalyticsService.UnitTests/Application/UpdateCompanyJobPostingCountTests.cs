using FlashMediator;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.AnalyticsService.Application.Exceptions;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.UpdateCompanyJobPostingCount;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Application.UnitOfWork;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Infrastructure.Messages;
using Vettingo.AnalyticsService.Infrastructure.Messaging;

namespace Vettingo.AnalyticsService.UnitTests.Application;

public sealed class UpdateCompanyJobPostingCountTests
{
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAnalyticsRepository analytics = Substitute.For<IAnalyticsRepository>();
    private readonly IProcessedEventRepository events = Substitute.For<IProcessedEventRepository>();
    private readonly UpdateCompanyJobPostingCountCommandRequest request = new()
    {
        EventId = Guid.CreateVersion7(), CompanyId = Guid.NewGuid()
    };

    public UpdateCompanyJobPostingCountTests()
    {
        unitOfWork.AnalyticsRepository.Returns(analytics);
        unitOfWork.ProcessedEventRepository.Returns(events);
        analytics.IncrementCompanyJobPostingCountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(1);
    }

    [Fact]
    public async Task New_Event_Should_Save_Marker_Before_Increment_And_Commit_The_Same_Transaction()
    {
        using var cancellation = new CancellationTokenSource();
        await Handler().Handle(request, cancellation.Token);

        Received.InOrder(() =>
        {
            _ = events.ExistsAsync(request.EventId, cancellation.Token);
            _ = unitOfWork.BeginTransactionAsync(cancellation.Token);
            _ = events.AddAsync(Arg.Is<ProcessedEvent>(item => item.EventId == request.EventId), cancellation.Token);
            _ = unitOfWork.SaveChangesAsync(cancellation.Token);
            _ = analytics.IncrementCompanyJobPostingCountAsync(request.CompanyId, cancellation.Token);
            _ = unitOfWork.CommitTransactionAsync(cancellation.Token);
        });
        await unitOfWork.DidNotReceiveWithAnyArgs().RollbackTransactionAsync(default);
    }

    [Fact]
    public async Task Processed_Event_Should_Return_Without_Transaction_Or_Increment()
    {
        events.ExistsAsync(request.EventId, CancellationToken.None).Returns(true);

        await Handler().Handle(request, CancellationToken.None);

        await unitOfWork.DidNotReceiveWithAnyArgs().BeginTransactionAsync(default);
        await events.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        analytics.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Repeated_Delivery_Should_Increment_Only_Once()
    {
        bool committed = false;
        events.ExistsAsync(request.EventId, CancellationToken.None).Returns(_ => committed);
        unitOfWork.CommitTransactionAsync(CancellationToken.None).Returns(_ =>
        {
            committed = true;
            return Task.CompletedTask;
        });

        await Handler().Handle(request, CancellationToken.None);
        await Handler().Handle(request, CancellationToken.None);

        await analytics.Received(1).IncrementCompanyJobPostingCountAsync(request.CompanyId, CancellationToken.None);
        await events.Received(1).AddAsync(Arg.Any<ProcessedEvent>(), CancellationToken.None);
        await unitOfWork.Received(1).CommitTransactionAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Duplicate_Primary_Key_During_Save_Should_Roll_Back_Before_Increment()
    {
        unitOfWork.SaveChangesAsync(CancellationToken.None)
            .Returns(Task.FromException<int>(new DbUpdateException("Duplicate EventId")));
        Func<Task> action = () => Handler().Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<DbUpdateException>();
        analytics.ReceivedCalls().Should().BeEmpty();
        await AssertRollback();
    }

    [Fact]
    public async Task Missing_Company_Statistic_Should_Roll_Back_The_Event_Marker()
    {
        analytics.IncrementCompanyJobPostingCountAsync(request.CompanyId, CancellationToken.None).Returns(0);
        Func<Task> action = () => Handler().Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
        await AssertRollback();
    }

    [Fact]
    public async Task Failed_Increment_Should_Roll_Back_The_Event_Marker()
    {
        analytics.IncrementCompanyJobPostingCountAsync(request.CompanyId, CancellationToken.None)
            .Returns(Task.FromException<int>(new InvalidOperationException("Update failed")));
        Func<Task> action = () => Handler().Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();
        await AssertRollback();
    }

    [Fact]
    public async Task Failed_Commit_Should_Roll_Back()
    {
        unitOfWork.CommitTransactionAsync(CancellationToken.None)
            .Returns(Task.FromException(new InvalidOperationException("Commit failed")));
        Func<Task> action = () => Handler().Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();
        await unitOfWork.Received(1).RollbackTransactionAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Cancellation_Should_Still_Roll_Back_With_An_Uncancelled_Token()
    {
        using var cancellation = new CancellationTokenSource();
        analytics.IncrementCompanyJobPostingCountAsync(request.CompanyId, cancellation.Token).Returns(call =>
        {
            cancellation.Cancel();
            return Task.FromCanceled<int>(cancellation.Token);
        });
        Func<Task> action = () => Handler().Handle(request, cancellation.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
        await AssertRollback();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Empty_Identifiers_Should_Not_Start_Processing(bool emptyEventId)
    {
        var invalid = request with
        {
            EventId = emptyEventId ? Guid.Empty : request.EventId,
            CompanyId = emptyEventId ? request.CompanyId : Guid.Empty
        };
        Func<Task> action = () => Handler().Handle(invalid, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequestException>();
        events.ReceivedCalls().Should().BeEmpty();
        await unitOfWork.DidNotReceiveWithAnyArgs().BeginTransactionAsync(default);
    }

    [Fact]
    public async Task Consumer_Should_Forward_EventId_And_CompanyId_To_Command()
    {
        var mediator = Substitute.For<IMediator>();
        using var cancellation = new CancellationTokenSource();

        await new JobAnalyticsConsumer(mediator).HandleAsync(new JobAnalyticsMessage
        {
            EventId = request.EventId, CompanyId = request.CompanyId
        }, cancellation.Token);

        await mediator.Received(1).Send(Arg.Is<UpdateCompanyJobPostingCountCommandRequest>(command =>
            command.EventId == request.EventId && command.CompanyId == request.CompanyId), cancellation.Token);
    }

    private UpdateCompanyJobPostingCountCommandHandler Handler() => new(unitOfWork);

    private async Task AssertRollback()
    {
        await unitOfWork.Received(1).RollbackTransactionAsync(CancellationToken.None);
        await unitOfWork.DidNotReceiveWithAnyArgs().CommitTransactionAsync(default);
    }
}
