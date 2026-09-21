using FluentAssertions;
using NSubstitute;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Infrastructure.Messaging;

namespace Vettingo.JobService.UnitTests.Messaging;

public class JobApplicationCreatedConsumerTests
{
    [Fact]
    public async Task Handle_ShouldFindPostingIncrementThroughDomainAndSave()
    {
        var repository = Substitute.For<IJobPostingRepository>();
        var posting = new JobPosting();
        posting.SetId();
        repository.GetJobPostingByIdAsync(posting.Id).Returns(posting);
        await new JobApplicationCreatedConsumer(repository).HandleAsync(posting.Id);
        posting.ApplicationCount.Should().Be(1);
        posting.UpdatedAt.Should().NotBeNull();
        await repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_WhenPostingIsMissing_ShouldNotSave()
    {
        var repository = Substitute.For<IJobPostingRepository>();
        Func<Task> act = () => new JobApplicationCreatedConsumer(repository).HandleAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
        await repository.DidNotReceive().SaveChangesAsync();
    }
}
