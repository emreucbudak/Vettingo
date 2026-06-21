using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.DeleteJobPosting
{
    public class DeleteJobPostingCommandHandler(IJobPostingRepository jobPostingRepository, ILogger<DeleteJobPostingCommandHandler> logger) : IRequestHandler<DeleteJobPostingCommandRequest>
    {
        public async Task Handle(DeleteJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteJobPostingCommandHandler));
            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new NotFoundException("İş ilanı bulunamadı");
            }

            jobPostingRepository.DeleteJobPosting(jobPosting);
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}



