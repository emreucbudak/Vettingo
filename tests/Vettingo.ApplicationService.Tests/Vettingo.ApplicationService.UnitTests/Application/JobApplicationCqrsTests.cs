using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ApplicationService.Application.Exceptions;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Query.GetAll;
using Vettingo.ApplicationService.Application.Repository;
using Vettingo.ApplicationService.Domain.Entities;
using Vettingo.ApplicationService.Domain.Enums;

namespace Vettingo.ApplicationService.UnitTests.Application
{
    public class JobApplicationCqrsTests
    {
        [Fact]
        public async Task CreateHandler_ShouldPersistApplication()
        {
            var repository = Substitute.For<IJobApplicationRepository>();
            repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
            var handler = new CreateJobApplicationCommandHandler(
                repository,
                Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
            var request = new CreateJobApplicationCommandRequest
            {
                CandidateId = Guid.NewGuid(),
                JobPostingId = Guid.NewGuid(),
                AppliedAt = DateTime.UtcNow,
                Status = ApplicationStatus.Submitted
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Id.Should().NotBeEmpty();
            await repository.Received(1).AddAsync(Arg.Is<JobApplication>(application =>
                application.CandidateId == request.CandidateId &&
                application.JobPostingId == request.JobPostingId));
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateHandler_WhenDuplicate_ShouldThrowBadRequest()
        {
            var repository = Substitute.For<IJobApplicationRepository>();
            repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
            var handler = new CreateJobApplicationCommandHandler(
                repository,
                Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());

            Func<Task> action = () => handler.Handle(new CreateJobApplicationCommandRequest
            {
                CandidateId = Guid.NewGuid(),
                JobPostingId = Guid.NewGuid()
            }, CancellationToken.None);

            await action.Should().ThrowAsync<BadRequestException>();
            await repository.DidNotReceive().AddAsync(Arg.Any<JobApplication>());
        }

        [Fact]
        public async Task GetAllHandler_ShouldMapApplications()
        {
            var repository = Substitute.For<IJobApplicationRepository>();
            JobApplication application = new();
            application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.UnderReview);
            repository.GetAllAsync(application.CandidateId, null).Returns([application]);
            var handler = new GetJobApplicationsQueryHandler(
                repository,
                Substitute.For<ILogger<GetJobApplicationsQueryHandler>>());

            var response = (await handler.Handle(
                new GetJobApplicationsQueryRequest { CandidateId = application.CandidateId },
                CancellationToken.None)).Single();

            response.Id.Should().Be(application.Id);
            response.Status.Should().Be(ApplicationStatus.UnderReview);
        }
    }
}
