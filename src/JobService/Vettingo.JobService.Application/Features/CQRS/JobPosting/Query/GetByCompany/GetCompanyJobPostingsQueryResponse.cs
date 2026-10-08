namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetByCompany;

public record GetCompanyJobPostingsQueryResponse(
    Guid Id, string Title, int CityId, string CityName, int Applicants, DateTime? PublishedAt, string Status);
