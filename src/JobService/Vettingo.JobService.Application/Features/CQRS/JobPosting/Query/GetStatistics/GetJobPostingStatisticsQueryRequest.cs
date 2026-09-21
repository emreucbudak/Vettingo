using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetStatistics;

public sealed record GetJobPostingStatisticsQueryRequest(Guid CompanyId) : IRequest<JobPostingStatistics>;
