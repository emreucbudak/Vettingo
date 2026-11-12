using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.UnitOfWork;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

public class GetAllPersonalizedJobPostingsQueryHandler(
    IJobUnitOfWork unitOfWork,
    ILogger<GetAllPersonalizedJobPostingsQueryHandler> logger)
    : IRequestHandler<GetAllPersonalizedJobPostingsQueryRequest, IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    public async Task<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>> Handle(
        GetAllPersonalizedJobPostingsQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllPersonalizedJobPostingsQueryHandler));
        var postings = await unitOfWork.PersonalizedJobPostings.AsNoTracking()
            .Include(posting => posting.JobPosting).ThenInclude(jobPosting => jobPosting.City)
            .Where(posting => posting.UserId == request.UserId)
            .OrderByDescending(posting => posting.CreatedAt)
            .ThenByDescending(posting => posting.Id)
            .ToListAsync(cancellationToken);
        return postings.Select(posting => new GetAllPersonalizedJobPostingsQueryResponse
        {
            JobPostingId = posting.JobPostingId,
            CompanyId = posting.JobPosting.CompanyId,
            Title = posting.JobPosting.Title,
            CityName = posting.JobPosting.City.CityName,
            EmploymentType = posting.JobPosting.EmploymentType,
            WorkingModel = posting.JobPosting.WorkingModel,
            MinimumExperienceYear = posting.JobPosting.MinimumExperienceYear,
            MaximumExperienceYear = posting.JobPosting.MaximumExperienceYear,
            MinSalary = posting.JobPosting.MinSalary,
            MaxSalary = posting.JobPosting.MaxSalary
        }).ToList();
    }
}
