using FlashMediator;
using System.Text.Json.Serialization;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryRequest : IRequest<GetJobStageExamByJobIdQueryResponse>
    {
        public Guid JobId { get; init; }
        public Guid ExamId { get; init; }

        [JsonIgnore]
        public Guid UserId { get; init; }
    }
}
