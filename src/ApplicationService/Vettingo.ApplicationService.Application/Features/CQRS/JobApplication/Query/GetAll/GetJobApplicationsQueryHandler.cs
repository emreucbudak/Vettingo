using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ApplicationService.Application.Repository;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Query.GetAll
{
    public class GetJobApplicationsQueryHandler(
        IJobApplicationRepository repository,
        ILogger<GetJobApplicationsQueryHandler> logger)
        : IRequestHandler<GetJobApplicationsQueryRequest, IEnumerable<GetJobApplicationsQueryResponse>>
    {
        public async Task<IEnumerable<GetJobApplicationsQueryResponse>> Handle(
            GetJobApplicationsQueryRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetJobApplicationsQueryHandler));

            var applications = await repository.GetAllAsync(request.CandidateId, request.JobPostingId);
            return applications.Select(application => new GetJobApplicationsQueryResponse
            {
                Id = application.Id,
                CandidateId = application.CandidateId,
                JobPostingId = application.JobPostingId,
                AppliedAt = application.AppliedAt,
                Status = application.Status,
                CreatedAt = application.CreatedAt,
                UpdatedAt = application.UpdatedAt
            });
        }
    }
}
