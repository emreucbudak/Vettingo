using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll
{
    public class GetAllJobPostingsQueryHandler(IJobPostingRepository jobPostingRepository) : IRequestHandler<GetAllJobPostingsQueryRequest, IEnumerable<GetAllJobPostingsQueryResponse>>
    {
        public async Task<IEnumerable<GetAllJobPostingsQueryResponse>> Handle(GetAllJobPostingsQueryRequest request, CancellationToken cancellationToken)
        {
            var jobPostings = request.CompanyId.HasValue
                ? await jobPostingRepository.GetJobPostingsByCompanyIdAsync(request.CompanyId.Value)
                : await jobPostingRepository.GetAllJobPostingsAsync();

            return jobPostings.Select(jobPosting => new GetAllJobPostingsQueryResponse
            {
                Id = jobPosting.Id,
                CompanyId = jobPosting.CompanyId,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                WorkingModel = jobPosting.WorkingModel,
                ExperienceLevel = jobPosting.ExperienceLevel,
                MinSalary = jobPosting.MinSalary,
                MaxSalary = jobPosting.MaxSalary,
                ApplicationDeadline = jobPosting.ApplicationDeadline,
                Status = jobPosting.Status,
                CreatedAt = jobPosting.CreatedAt
            });
        }
    }
}
