using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;

public class GetLatestPersonalizedJobPostingsQueryHandler(
    IPersonalizedJobPostingsRepository repository,
    ILogger<GetLatestPersonalizedJobPostingsQueryHandler> logger)
    : IRequestHandler<GetLatestPersonalizedJobPostingsQueryRequest, IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    public async Task<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>> Handle(
        GetLatestPersonalizedJobPostingsQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetLatestPersonalizedJobPostingsQueryHandler));
        var postings = await repository.GetLatestAsync(request.UserId, cancellationToken);
        return postings.Select(posting => new GetAllPersonalizedJobPostingsQueryResponse
        {
            Id = posting.Id,
            UserId = posting.UserId,
            JobPostingId = posting.JobPostingId,
            JobPosting = new GetJobPostingByIdQueryResponse
            {
                Id = posting.JobPosting.Id,
                CompanyId = posting.JobPosting.CompanyId,
                Title = posting.JobPosting.Title,
                RequirementSkills = new Dictionary<string, int>(posting.JobPosting.RequirementSkills),
                CityId = posting.JobPosting.CityId,
                EmploymentType = posting.JobPosting.EmploymentType,
                WorkingModel = posting.JobPosting.WorkingModel,
                MinimumExperienceYear = posting.JobPosting.MinimumExperienceYear,
                MaximumExperienceYear = posting.JobPosting.MaximumExperienceYear,
                MinSalary = posting.JobPosting.MinSalary,
                MaxSalary = posting.JobPosting.MaxSalary,
                ApplicationDeadline = posting.JobPosting.ApplicationDeadline,
                Status = posting.JobPosting.Status,
                CreatedAt = posting.JobPosting.CreatedAt,
                UpdatedAt = posting.JobPosting.UpdatedAt
            },
            CreatedAt = posting.CreatedAt,
            UpdatedAt = posting.UpdatedAt
        }).ToList();
    }
}
