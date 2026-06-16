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
                JobId = exam.JobId,
                Title = exam.Title,
                Subject = exam.Subject,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                IsActive = exam.IsActive,
                QuestionCount = exam.Questions.Count,
                CreatedAt = exam.CreatedAt
            });
        }
    }
}
