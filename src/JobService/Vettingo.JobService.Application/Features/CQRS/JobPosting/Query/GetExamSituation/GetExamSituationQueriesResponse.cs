namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetExamSituation
{
    public record GetExamSituationQueriesResponse
    {
        public bool InUse { get; init; }
    }
}
