using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CloseJobPosting
{
    public class CloseJobPostingCommandHandler(IJobPostingRepository jobPostingRepository, ILogger<CloseJobPostingCommandHandler> logger) : IRequestHandler<CloseJobPostingCommandRequest>
    {
        public async Task Handle(CloseJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CloseJobPostingCommandHandler));
            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new NotFoundException("İş ilanı bulunamadı");
            }

            jobPosting.CloseJobPosting();
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}
