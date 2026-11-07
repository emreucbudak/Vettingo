using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CloseJobPostingApplications
{
    public class CloseJobPostingApplicationsCommandHandler(
        IJobPostingRepository jobPostingRepository,
        ILogger<CloseJobPostingApplicationsCommandHandler> logger) : IRequestHandler<CloseJobPostingApplicationsCommandRequest>
    {
        public async Task Handle(CloseJobPostingApplicationsCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CloseJobPostingApplicationsCommandHandler));
            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new NotFoundException("İş ilanı bulunamadı");
            }

            jobPosting.CloseJobPostingApplications();
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}
