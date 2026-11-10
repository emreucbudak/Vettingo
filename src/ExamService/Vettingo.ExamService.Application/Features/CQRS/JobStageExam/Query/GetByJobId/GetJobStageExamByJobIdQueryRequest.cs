using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryRequest : IRequest<GetJobStageExamByJobIdQueryResponse>
    {
        public Guid JobId { get; init; }
    }
}
