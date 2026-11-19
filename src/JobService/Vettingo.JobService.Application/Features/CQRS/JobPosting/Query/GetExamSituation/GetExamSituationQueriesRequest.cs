using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetExamSituation
{
    public record GetExamSituationQueriesRequest : IRequest<GetExamSituationQueriesResponse>
    {
        public Guid ExamId { get; init; }
    }
}
