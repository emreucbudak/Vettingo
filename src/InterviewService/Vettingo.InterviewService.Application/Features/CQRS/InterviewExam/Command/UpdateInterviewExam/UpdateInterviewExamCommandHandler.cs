using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Domain.Enums;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.UpdateInterviewExam
{
    public class UpdateInterviewExamCommandHandler(IRepository<InterviewExamEntity> examRepository, IRepository<InterviewQuestionEntity> questionRepository, ILogger<UpdateInterviewExamCommandHandler> logger) : IRequestHandler<UpdateInterviewExamCommandRequest>
    {
        public async Task Handle(UpdateInterviewExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateInterviewExamCommandHandler));

            InterviewExamEntity exam = await examRepository.GetByIdAsync(request.InterviewExamId, "Questions.InterviewQuestion")
                ?? throw new NotFoundException("Mülakat bulunamadı");

            Guid[] distinctQuestionIds = request.QuestionIds.Distinct().ToArray();
            if (request.Type == InterviewType.AI && distinctQuestionIds.Length == 0)
            {
                throw new BadRequestException("AI mülakatında en az bir soru olmalıdır");
            }

            if (distinctQuestionIds.Length > 0)
            {
                int questionCount = await questionRepository.CountAsync(question => distinctQuestionIds.Contains(question.Id) && (question.CompanyId == null || question.CompanyId == exam.CompanyId));
                if (questionCount != distinctQuestionIds.Length)
                {
                    throw new BadRequestException("Mülakat için seçilen sorulardan bazıları bulunamadı veya bu şirkete ait değil");
                }
            }

            exam.UpdateExam(
                request.CandidateId,
                request.Title,
                request.Description,
                request.Type,
                request.StartDate,
                request.EndDate,
                distinctQuestionIds);
            examRepository.Update(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}
