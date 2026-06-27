using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam;
using Vettingo.InterviewService.Application.Repository;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetById
{
    public class GetInterviewExamByIdQueryHandler(IRepository<InterviewExamEntity> examRepository, ILogger<GetInterviewExamByIdQueryHandler> logger) : IRequestHandler<GetInterviewExamByIdQueryRequest, GetInterviewExamByIdQueryResponse>
    {
        public async Task<GetInterviewExamByIdQueryResponse> Handle(GetInterviewExamByIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetInterviewExamByIdQueryHandler));

            InterviewExamEntity exam = await examRepository.GetByIdAsync(request.InterviewExamId, "Questions.InterviewQuestion")
                ?? throw new NotFoundException("Mülakat sınavı bulunamadı");

            return new GetInterviewExamByIdQueryResponse
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
            };
        }
    }
}


