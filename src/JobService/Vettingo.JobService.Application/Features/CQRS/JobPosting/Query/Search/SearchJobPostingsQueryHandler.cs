using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.Search;

public sealed class SearchJobPostingsQueryHandler(
    IJobPostingRepository jobPostingRepository,
    ILogger<SearchJobPostingsQueryHandler> logger)
    : IRequestHandler<SearchJobPostingsQueryRequest, IReadOnlyList<SearchJobPostingsQueryResponse>>
{
    public async Task<IReadOnlyList<SearchJobPostingsQueryResponse>> Handle(
        SearchJobPostingsQueryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(SearchJobPostingsQueryHandler));
        var criteria = new JobPostingSearchCriteria
        {
            Title = request.Title,
            Location = request.Location,
            EmploymentType = request.EmploymentType,
            WorkingModel = request.WorkingModel,
            ExperienceLevel = request.ExperienceLevel,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary
        };
        var jobPostings = await jobPostingRepository.SearchJobPostingsAsync(criteria, cancellationToken);
        return jobPostings.Select(jobPosting => new SearchJobPostingsQueryResponse
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
        }).ToArray();
    }
}
