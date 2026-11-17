using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.UnitOfWork;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll
{
    public class GetAllExamsQueryHandler(IExamUnitOfWork unitOfWork, ILogger<GetAllExamsQueryHandler> logger) : IRequestHandler<GetAllExamsQueryRequest, IEnumerable<GetAllExamsQueryResponse>>
    {
        public async Task<IEnumerable<GetAllExamsQueryResponse>> Handle(GetAllExamsQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllExamsQueryHandler));
            var exams = await unitOfWork.ExamRepository.GetAllAsync();

            return exams.Select(exam => new GetAllExamsQueryResponse
            {
                Id = exam.Id,
                CompanyId = exam.CompanyId,
                OwnerType = exam.OwnerType,
                Title = exam.Title,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                MultipleChoiceQuestionCount = exam.MultipleChoiceQuestions.Count,
                TrueFalseQuestionCount = exam.TrueFalseQuestions.Count,
                CreatedAt = exam.CreatedAt
            });
        }
    }
}


