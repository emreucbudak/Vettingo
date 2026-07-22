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

            Guid? companyId = request.CompanyId;
            Guid? candidateId = request.CandidateId;
            DateTime now = DateTime.UtcNow;
            Expression<Func<InterviewExamEntity, bool>>? predicate = null;

            if (companyId.HasValue || candidateId.HasValue || request.UpcomingOnly)
            {
                predicate = exam =>
                    (!companyId.HasValue || exam.CompanyId == companyId.Value) &&
                    (!candidateId.HasValue || exam.CandidateId == candidateId.Value) &&
                    (!request.UpcomingOnly || exam.StartDate >= now);
            }

            IEnumerable<InterviewExamEntity> exams = await examRepository.GetAllAsync(
                predicate,
                query => query.OrderBy(exam => exam.StartDate),
                "Questions.InterviewQuestion");

            return exams.Select(exam => new GetAllInterviewExamsQueryResponse
            {
                Id = exam.Id,
                CompanyId = exam.CompanyId,
                CandidateId = exam.CandidateId,
                Title = exam.Title,
                Description = exam.Description,
                Type = exam.Type,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
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
