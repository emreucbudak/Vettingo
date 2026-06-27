using System.Linq.Expressions;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Repository;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetAll
{
    public class GetAllInterviewQuestionsQueryHandler(IRepository<InterviewQuestionEntity> questionRepository, ILogger<GetAllInterviewQuestionsQueryHandler> logger) : IRequestHandler<GetAllInterviewQuestionsQueryRequest, IEnumerable<GetAllInterviewQuestionsQueryResponse>>
    {
        public async Task<IEnumerable<GetAllInterviewQuestionsQueryResponse>> Handle(GetAllInterviewQuestionsQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllInterviewQuestionsQueryHandler));

            Expression<Func<InterviewQuestionEntity, bool>>? predicate = null;
            if (request.CompanyId.HasValue)
            {
                Guid companyId = request.CompanyId.Value;
                predicate = question => question.CompanyId == null || question.CompanyId == companyId;
            }

            IEnumerable<InterviewQuestionEntity> questions = await questionRepository.GetAllAsync(predicate, query => query.OrderByDescending(question => question.CreatedAt));
            return questions.Select(question => new GetAllInterviewQuestionsQueryResponse
            {
                Id = question.Id,
                CompanyId = question.CompanyId,
                QuestionText = question.QuestionText,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt
            });
        }
    }
}


