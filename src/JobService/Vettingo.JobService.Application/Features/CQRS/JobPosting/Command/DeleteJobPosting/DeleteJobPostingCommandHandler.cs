using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.DeleteJobPosting
{
    public class DeleteJobPostingCommandHandler(IJobPostingRepository jobPostingRepository) : IRequestHandler<DeleteJobPostingCommandRequest>
    {
        public async Task Handle(DeleteJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new Exception("Job posting not found");
            }

            jobPostingRepository.DeleteJobPosting(jobPosting);
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}
