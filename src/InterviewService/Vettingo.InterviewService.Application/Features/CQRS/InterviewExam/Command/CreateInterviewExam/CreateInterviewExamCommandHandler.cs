using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Domain.Enums;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam
{
    public class CreateInterviewExamCommandHandler(IRepository<InterviewExamEntity> examRepository, IRepository<InterviewQuestionEntity> questionRepository, ILogger<CreateInterviewExamCommandHandler> logger) : IRequestHandler<CreateInterviewExamCommandRequest>
    {
        public async Task Handle(CreateInterviewExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateInterviewExamCommandHandler));

            Guid[] distinctQuestionIds = request.QuestionIds.Distinct().ToArray();
            if (request.Type == InterviewType.AI && distinctQuestionIds.Length == 0)
            {
                throw new BadRequestException("AI mülakatında en az bir soru olmalıdır");
            }

            if (distinctQuestionIds.Length > 0)
            {
                int questionCount = await questionRepository.CountAsync(question => distinctQuestionIds.Contains(question.Id) && (question.CompanyId == null || question.CompanyId == request.CompanyId));
                if (questionCount != distinctQuestionIds.Length)
                {
                    throw new BadRequestException("Mülakat için seçilen sorulardan bazıları bulunamadı veya bu şirkete ait değil");
                }
            }

            InterviewExamEntity exam = new();
            exam.CreateExam(
                request.CompanyId,
                request.CandidateId,
                request.Title,
                request.Description,
                request.Type,
                request.StartDate,
                request.EndDate,
                distinctQuestionIds);

            await examRepository.AddAsync(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}
