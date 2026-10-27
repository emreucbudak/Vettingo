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
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Application.Validations;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Infrastructure.Messaging;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class PersonalizedJobPostingMessagingTests
{
    [Fact]
    public async Task Published_Dto_Should_Contain_Only_Ids_And_Load_Referenced_JobPosting()
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
        var jobPosting = new JobPosting();
        jobPosting.CreateJobPosting(Guid.NewGuid(), "Backend Developer", new Dictionary<string, int> { ["C#"] = 3 },
            34, EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 65000, 85000, null, JobPostingStatus.Active);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        var cap = Substitute.For<ICapPublisher>();
        cap.PublishAsync(Arg.Any<string>(), Arg.Any<PersonalizedJobPostingDto>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var publisher = new PersonalizedJobPostingPublisher(cap);
        var dto = new PersonalizedJobPostingDto
        {
            UserId = Guid.NewGuid(),
            JobPostingId = jobPosting.Id
        };
        using var cancellation = new CancellationTokenSource();
        await publisher.PublishAsync(dto, cancellation.Token);

        var publishedCall = cap.ReceivedCalls().Single();
        publishedCall.GetArguments()[0].Should().Be(PersonalizedJobPostingMessage.TopicName);
        publishedCall.GetArguments()[3].Should().Be(cancellation.Token);
        var json = JsonSerializer.Serialize(publishedCall.GetArguments()[1], JsonSerializerOptions.Web);
        using var document = JsonDocument.Parse(json);
        document.RootElement.EnumerateObject().Select(property => property.Name)
            .Should().BeEquivalentTo("userId", "jobPostingId");
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
        var persisted = (await all.Handle(new() { UserId = dto.UserId }, cancellation.Token))
            .Should().ContainSingle().Subject;
        persisted.UserId.Should().Be(dto.UserId);
        persisted.JobPostingId.Should().Be(dto.JobPostingId);
        persisted.JobPosting.Id.Should().Be(jobPosting.Id);
        persisted.JobPosting.Title.Should().Be(jobPosting.Title);
        persisted.JobPosting.CityId.Should().Be(jobPosting.CityId);
        persisted.JobPosting.MinSalary.Should().Be(jobPosting.MinSalary);
        persisted.JobPosting.MaxSalary.Should().Be(jobPosting.MaxSalary);
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
            UserId = Guid.Empty, JobPostingId = Guid.Empty
        };

        Func<Task> consume = () => consumer.HandleAsync(message, CancellationToken.None);
        var failure = await consume.Should().ThrowAsync<ValidationException>();
        failure.Which.Errors.Select(error => error.PropertyName).Should()
            .BeEquivalentTo("UserId", "JobPostingId");
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
            UserId = Guid.NewGuid(), JobPostingId = Guid.NewGuid()
        };

        Func<Task> consume = () => consumer.HandleAsync(message, CancellationToken.None);
        await consume.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database unavailable");
    }
}
