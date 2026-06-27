using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById
{
    public class GetInterviewQuestionByIdQueryHandler(IRepository<InterviewQuestionEntity> questionRepository, ILogger<GetInterviewQuestionByIdQueryHandler> logger) : IRequestHandler<GetInterviewQuestionByIdQueryRequest, GetInterviewQuestionByIdQueryResponse>
    {
        public async Task<GetInterviewQuestionByIdQueryResponse> Handle(GetInterviewQuestionByIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetInterviewQuestionByIdQueryHandler));

            InterviewQuestionEntity question = await questionRepository.GetByIdAsync(request.InterviewQuestionId)
                ?? throw new NotFoundException("Mülakat sorusu bulunamadı");

            return new GetInterviewQuestionByIdQueryResponse
            {
                Id = question.Id,
                CompanyId = question.CompanyId,
                QuestionText = question.QuestionText,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt
            };
        }
    }
}


