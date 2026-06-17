using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting
{
    public class CreateJobPostingCommandHandler(IJobPostingRepository jobPostingRepository) : IRequestHandler<CreateJobPostingCommandRequest>
    {
        public async Task Handle(CreateJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                request.CompanyId,
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
                request.ApplicationDeadline,
                request.Status);

            await jobPostingRepository.AddJobPostingAsync(jobPosting);
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}
