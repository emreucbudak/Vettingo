using System.Linq.Expressions;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Repository;
using InterviewAnswerEntity = Vettingo.InterviewService.Domain.Entities.InterviewAnswer;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Query.GetAll
{
    public class GetInterviewAnswersQueryHandler(IRepository<InterviewAnswerEntity> answerRepository, ILogger<GetInterviewAnswersQueryHandler> logger) : IRequestHandler<GetInterviewAnswersQueryRequest, IEnumerable<GetInterviewAnswersQueryResponse>>
    {
        public async Task<IEnumerable<GetInterviewAnswersQueryResponse>> Handle(GetInterviewAnswersQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetInterviewAnswersQueryHandler));

            Expression<Func<InterviewAnswerEntity, bool>>? predicate = null;
            if (request.UserId.HasValue && request.InterviewExamId.HasValue)
            {
                Guid userId = request.UserId.Value;
                Guid interviewExamId = request.InterviewExamId.Value;
                predicate = answer => answer.UserId == userId && answer.InterviewExamId == interviewExamId;
            }
            else if (request.UserId.HasValue)
            {
                Guid userId = request.UserId.Value;
                predicate = answer => answer.UserId == userId;
            }
            else if (request.InterviewExamId.HasValue)
            {
                Guid interviewExamId = request.InterviewExamId.Value;
                predicate = answer => answer.InterviewExamId == interviewExamId;
            }

            IEnumerable<InterviewAnswerEntity> answers = await answerRepository.GetAllAsync(predicate, query => query.OrderByDescending(answer => answer.AnswerDate));
            return answers.Select(answer => new GetInterviewAnswersQueryResponse
            {
                Id = answer.Id,
                UserId = answer.UserId,
                InterviewExamId = answer.InterviewExamId,
                AnswerDate = answer.AnswerDate,
                CreatedAt = answer.CreatedAt
            });
        }
    }
}


