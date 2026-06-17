using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting
{
    public class UpdateJobPostingCommandHandler(IJobPostingRepository jobPostingRepository) : IRequestHandler<UpdateJobPostingCommandRequest>
    {
        public async Task Handle(UpdateJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new Exception("Job posting not found");
            }

            jobPosting.UpdateJobPosting(
                request.Title,
                request.Description,
                request.Requirements,
                request.Responsibilities,
                request.Location,
                request.EmploymentType,
                request.WorkingModel,
                request.ExperienceLevel,
                request.MinSalary,
                request.MaxSalary,
                request.ApplicationDeadline);
            jobPosting.SetStatus(request.Status);

            jobPostingRepository.UpdateJobPosting(jobPosting);
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}
