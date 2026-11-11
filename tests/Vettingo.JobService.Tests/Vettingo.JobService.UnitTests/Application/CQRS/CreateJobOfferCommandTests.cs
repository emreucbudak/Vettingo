using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.JobService.Application.Features.CQRS.JobOffer.Command.Create;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.UnitTests.Application.CQRS;

public sealed class CreateJobOfferCommandTests
{
    [Fact]
    public async Task Create_Should_Track_New_Offer_And_Remove_Candidate_In_The_Same_Save()
    {
        using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused").Options);
        var jobPostingId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var otherCandidateId = Guid.NewGuid();
        var offers = new Offers();
        var startDate = new DateOnly(2026, 11, 11);
        offers.CreateOffers(jobPostingId, startDate, startDate.AddDays(2), [candidateId, otherCandidateId]);
        context.Attach(offers);

        var repository = Substitute.For<IOffersRepository>();
        repository.GetByIdAsync(offers.Id, CancellationToken.None).Returns(offers);
        var unitOfWork = Substitute.For<IJobUnitOfWork>();
        unitOfWork.OffersRepository.Returns(repository);
        unitOfWork.SaveChangesAsync(CancellationToken.None).Returns(_ =>
        {
            context.ChangeTracker.DetectChanges();
            var offer = offers.JobOffers.Should().ContainSingle().Subject;
            context.Entry(offer).State.Should().Be(EntityState.Added);
            var candidateIds = context.Entry(offers).Property(stage => stage.CandidateIds);
            candidateIds.IsModified.Should().BeTrue();
            candidateIds.Metadata.IsConcurrencyToken.Should().BeTrue();
            return Task.FromResult(2);
        });
        var handler = new CreateJobOfferCommandHandler(unitOfWork);

        await handler.Handle(new CreateJobOfferCommandRequest
        {
            UserId = candidateId,
            OffersId = offers.Id,
            JobPostingId = jobPostingId,
            Salary = 50_000
        }, CancellationToken.None);

        offers.CandidateIds.Should().Equal(otherCandidateId);
        offers.JobOffers.Should().ContainSingle().Which.Should().BeEquivalentTo(new
        {
            UserId = candidateId,
            OffersId = offers.Id,
            JobPostingId = jobPostingId,
            Salary = 50_000
        });
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
