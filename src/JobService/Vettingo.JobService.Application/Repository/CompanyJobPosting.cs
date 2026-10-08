using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Repository;

public sealed record CompanyJobPosting(
    Guid Id, string Title, int CityId, string CityName, int Applicants, DateTime CreatedAt, JobPostingStatus Status);
