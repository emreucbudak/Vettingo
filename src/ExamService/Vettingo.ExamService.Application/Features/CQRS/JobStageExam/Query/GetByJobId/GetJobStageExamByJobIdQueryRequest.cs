using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryRequest : IRequest<List<GetJobStageExamByJobIdQueryResponse>>
    {
        public Guid JobId { get; init; }
        public Guid ExamId { get; init; }
    }
}
