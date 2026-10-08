using System.Text.Json;
using DotNetCore.CAP;
using FlashMediator;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.EvaluationService.Application.DTOs;
using Vettingo.EvaluationService.Infrastructure.Messaging;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Application.Validations;
using Vettingo.JobService.Infrastructure.Messaging;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class PersonalizedJobPostingMessagingTests
{
    [Fact]
    public async Task Published_Dto_Should_Be_Consumed_By_Existing_Handler_And_Return_Salary()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<JobDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<IPersonalizedJobPostingsRepository, PersonalizedJobPostingsRepository>();
        services.AddFlashMediator(typeof(CreatePersonalizedJobPostingCommandHandler).Assembly);
        services.AddFlashMediatorHybridCache();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<JobDbContext>();
        await context.Database.EnsureCreatedAsync();

        var cap = Substitute.For<ICapPublisher>();
        cap.PublishAsync(Arg.Any<string>(), Arg.Any<PersonalizedJobPostingDto>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var publisher = new PersonalizedJobPostingPublisher(cap);
        var dto = new PersonalizedJobPostingDto
        {
            UserId = Guid.NewGuid(),
            Title = "Backend Developer",
            PublishedDate = new DateOnly(2026, 10, 8),
            CityId = 34,
            Salary = 65000
        };
        using var cancellation = new CancellationTokenSource();
        await publisher.PublishAsync(dto, cancellation.Token);

        var publishedCall = cap.ReceivedCalls().Single();
        publishedCall.GetArguments()[0].Should().Be(PersonalizedJobPostingMessage.TopicName);
        publishedCall.GetArguments()[3].Should().Be(cancellation.Token);
        var json = JsonSerializer.Serialize(publishedCall.GetArguments()[1], JsonSerializerOptions.Web);
        var message = JsonSerializer.Deserialize<PersonalizedJobPostingMessage>(json, JsonSerializerOptions.Web)!;
        var consumer = new PersonalizedJobPostingRequestedConsumer(
            scope.ServiceProvider.GetRequiredService<IMediator>(),
            new CreatePersonalizedJobPostingCommandRequestValidator(),
            NullLogger<PersonalizedJobPostingRequestedConsumer>.Instance);

        await consumer.HandleAsync(message, cancellation.Token);
        context.ChangeTracker.Clear();
        var repository = scope.ServiceProvider.GetRequiredService<IPersonalizedJobPostingsRepository>();
        var all = new GetAllPersonalizedJobPostingsQueryHandler(repository,
            NullLogger<GetAllPersonalizedJobPostingsQueryHandler>.Instance);
        var latest = new GetLatestPersonalizedJobPostingsQueryHandler(repository,
            NullLogger<GetLatestPersonalizedJobPostingsQueryHandler>.Instance);
        var persisted = (await all.Handle(new() { UserId = dto.UserId }, cancellation.Token))
            .Should().ContainSingle().Subject;
        persisted.UserId.Should().Be(dto.UserId);
        persisted.Title.Should().Be(dto.Title);
        persisted.CityId.Should().Be(dto.CityId);
        persisted.CityName.Should().Be("İstanbul");
        persisted.PublishedDate.Should().Be(dto.PublishedDate);
        persisted.Salary.Should().Be(dto.Salary);
        (await latest.Handle(new() { UserId = dto.UserId }, cancellation.Token))
            .Should().ContainSingle().Which.Salary.Should().Be(dto.Salary);
    }

    [Fact]
    public async Task Invalid_Message_Should_Not_Dispatch_To_Handler()
    {
        var mediator = Substitute.For<IMediator>();
        var consumer = new PersonalizedJobPostingRequestedConsumer(mediator,
            new CreatePersonalizedJobPostingCommandRequestValidator(),
            NullLogger<PersonalizedJobPostingRequestedConsumer>.Instance);
        var message = new PersonalizedJobPostingMessage
        {
            UserId = Guid.Empty, Title = "", PublishedDate = default, CityId = 0, Salary = -1
        };

        Func<Task> consume = () => consumer.HandleAsync(message, CancellationToken.None);
        var failure = await consume.Should().ThrowAsync<ValidationException>();
        failure.Which.Errors.Select(error => error.PropertyName).Should()
            .BeEquivalentTo("UserId", "Title", "PublishedDate", "CityId", "Salary");
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Handler_Failure_Should_Propagate_To_CAP_For_Retry()
    {
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<IRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Database unavailable")));
        var consumer = new PersonalizedJobPostingRequestedConsumer(mediator,
            new CreatePersonalizedJobPostingCommandRequestValidator(),
            NullLogger<PersonalizedJobPostingRequestedConsumer>.Instance);
        var message = new PersonalizedJobPostingMessage
        {
            UserId = Guid.NewGuid(), Title = "Developer",
            PublishedDate = new DateOnly(2026, 10, 8), CityId = 34, Salary = 65000
        };

        Func<Task> consume = () => consumer.HandleAsync(message, CancellationToken.None);
        await consume.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database unavailable");
    }
}
