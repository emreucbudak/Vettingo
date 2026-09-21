using DotNetCore.CAP;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class JobApplicationCreatedConsumer(IJobPostingRepository repository) : ICapSubscribe
{
    [CapSubscribe("vettingo.job-application.created.v1", Group = "vettingo.job-service")]
    public async Task HandleAsync(Guid jobPostingId)
    {
        var jobPosting = await repository.GetJobPostingByIdAsync(jobPostingId)
            ?? throw new NotFoundException("İş ilanı bulunamadı.");

        jobPosting.IncrementApplicationCount();
        await repository.SaveChangesAsync();
    }
}
