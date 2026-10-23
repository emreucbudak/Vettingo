using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll
{
    public class GetAllExamsQueryHandler(IExamRepository examRepository, ILogger<GetAllExamsQueryHandler> logger) : IRequestHandler<GetAllExamsQueryRequest, IEnumerable<GetAllExamsQueryResponse>>
    {
        public async Task<IEnumerable<GetAllExamsQueryResponse>> Handle(GetAllExamsQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllExamsQueryHandler));
            var exams = await examRepository.GetAllExamsAsync();

            return exams.Select(exam => new GetAllExamsQueryResponse
            {
                Id = exam.Id,
                CompanyId = exam.CompanyId,
                JobId = exam.JobId,
                OwnerType = exam.OwnerType,
                Title = exam.Title,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                MultipleChoiceQuestionCount = exam.MultipleChoiceQuestions.Count,
                TrueFalseQuestionCount = exam.TrueFalseQuestions.Count,
                CreatedAt = exam.CreatedAt
            });
        }
    }
}


