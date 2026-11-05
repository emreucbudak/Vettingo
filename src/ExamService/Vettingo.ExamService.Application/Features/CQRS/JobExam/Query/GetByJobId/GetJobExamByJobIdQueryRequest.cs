using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.JobExam.Query.GetByJobId
{
    public class GetJobExamByJobIdQueryRequest : IRequest<GetJobExamByJobIdQueryResponse>
    {
        public Guid JobId { get; init; }
    }
}
