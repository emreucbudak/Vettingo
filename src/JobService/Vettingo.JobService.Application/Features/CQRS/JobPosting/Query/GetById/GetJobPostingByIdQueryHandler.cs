using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById
{
    public class GetJobPostingByIdQueryHandler(IJobPostingRepository jobPostingRepository, ILogger<GetJobPostingByIdQueryHandler> logger) : IRequestHandler<GetJobPostingByIdQueryRequest, GetJobPostingByIdQueryResponse>
    {
        public async Task<GetJobPostingByIdQueryResponse> Handle(GetJobPostingByIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetJobPostingByIdQueryHandler));
            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new NotFoundException("İş ilanı bulunamadı");
            }

            return new GetJobPostingByIdQueryResponse
            {
                Id = jobPosting.Id,
                CompanyId = jobPosting.CompanyId,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Requirements = jobPosting.Requirements,
                Responsibilities = jobPosting.Responsibilities,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                WorkingModel = jobPosting.WorkingModel,
                ExperienceLevel = jobPosting.ExperienceLevel,
                MinSalary = jobPosting.MinSalary,
                MaxSalary = jobPosting.MaxSalary,
                ApplicationDeadline = jobPosting.ApplicationDeadline,
                Status = jobPosting.Status,
                CreatedAt = jobPosting.CreatedAt,
                UpdatedAt = jobPosting.UpdatedAt
            };
        }
    }
}



