using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll
{
    public class GetAllExamsQueryHandler(IExamRepository examRepository) : IRequestHandler<GetAllExamsQueryRequest, IEnumerable<GetAllExamsQueryResponse>>
    {
        public async Task<IEnumerable<GetAllExamsQueryResponse>> Handle(GetAllExamsQueryRequest request, CancellationToken cancellationToken)
        {
            var exams = await examRepository.GetAllExamsAsync();

            return exams.Select(exam => new GetAllExamsQueryResponse
            {
                Id = exam.Id,
                CompanyId = exam.CompanyId,
                JobId = exam.JobId,
                OwnerType = exam.OwnerType,
                Title = exam.Title,
                Subject = exam.Subject,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                IsActive = exam.IsActive,
                MultipleChoiceQuestionCount = exam.MultipleChoiceQuestions.Count,
                TrueFalseQuestionCount = exam.TrueFalseQuestions.Count,
                ClassicQuestionCount = exam.ClassicQuestions.Count,
                CodeCompletionQuestionCount = exam.CodeCompletionQuestions.Count,
                CreatedAt = exam.CreatedAt
            });
        }
    }
}
