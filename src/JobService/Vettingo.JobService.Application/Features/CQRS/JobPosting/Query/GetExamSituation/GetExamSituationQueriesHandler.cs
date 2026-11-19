using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.UnitOfWork;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetExamSituation
{
    public class GetExamSituationQueriesHandler(IJobUnitOfWork unitOfWork) : IRequestHandler<GetExamSituationQueriesRequest, GetExamSituationQueriesResponse>
    {
        public async Task<GetExamSituationQueriesResponse> Handle(GetExamSituationQueriesRequest request, CancellationToken cancellationToken)
        {
            var steps = await unitOfWork.JobSteps.Where(a => a.ExamId ==  request.ExamId && a.StartedDate > DateOnly.FromDateTime(DateTime.UtcNow)).ToListAsync();
            if (steps.Any())
            {
                return new GetExamSituationQueriesResponse()
                {
                    InUse = true
                };
            }
            else
            {
                return new GetExamSituationQueriesResponse()
                {
                    InUse = false
                };
            }
        }
    }
}
