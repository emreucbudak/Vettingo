using FlashMediator;

using System.Text.Json.Serialization;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam
{
    public record DeleteExamCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }

        [JsonIgnore]
        public Guid CompanyId { get; init; }
    }
}
