using System.Linq.Expressions;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam;
using Vettingo.InterviewService.Application.Repository;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll
{
    public class GetAllInterviewExamsQueryHandler(IRepository<InterviewExamEntity> examRepository, ILogger<GetAllInterviewExamsQueryHandler> logger) : IRequestHandler<GetAllInterviewExamsQueryRequest, IEnumerable<GetAllInterviewExamsQueryResponse>>
    {
        public async Task<IEnumerable<GetAllInterviewExamsQueryResponse>> Handle(GetAllInterviewExamsQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllInterviewExamsQueryHandler));

            Expression<Func<InterviewExamEntity, bool>>? predicate = null;
            if (request.CompanyId.HasValue)
            {
                Guid companyId = request.CompanyId.Value;
                predicate = exam => exam.CompanyId == companyId;
            }

            IEnumerable<InterviewExamEntity> exams = await examRepository.GetAllAsync(predicate, query => query.OrderByDescending(exam => exam.CreatedAt), "Questions.InterviewQuestion");
            return exams.Select(exam => new GetAllInterviewExamsQueryResponse
            {
                Id = exam.Id,
                CompanyId = exam.CompanyId,
                Title = exam.Title,
                Description = exam.Description,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                Questions = exam.Questions
                    .OrderBy(examQuestion => examQuestion.DisplayOrder)
                    .Select(examQuestion => new InterviewExamQuestionResponse
                    {
                        InterviewQuestionId = examQuestion.InterviewQuestionId,
                        QuestionText = examQuestion.InterviewQuestion?.QuestionText ?? string.Empty,
                        DisplayOrder = examQuestion.DisplayOrder
                    })
            });
        }
    }
}


