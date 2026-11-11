using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.DTOs;

public sealed record CompanyJobPosting(
    Guid Id, string Title, int CityId, string CityName, int Applicants, DateTime CreatedAt, StepType? CurrentStepType);
