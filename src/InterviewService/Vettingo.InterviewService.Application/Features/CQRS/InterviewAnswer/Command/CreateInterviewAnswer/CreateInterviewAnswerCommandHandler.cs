using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;
using InterviewAnswerEntity = Vettingo.InterviewService.Domain.Entities.InterviewAnswer;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Command.CreateInterviewAnswer
{
    public class CreateInterviewAnswerCommandHandler(IRepository<InterviewAnswerEntity> answerRepository, IRepository<InterviewExamEntity> examRepository, ILogger<CreateInterviewAnswerCommandHandler> logger) : IRequestHandler<CreateInterviewAnswerCommandRequest>
    {
        public async Task Handle(CreateInterviewAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateInterviewAnswerCommandHandler));

            bool examExists = await examRepository.AnyAsync(exam => exam.Id == request.InterviewExamId);
            if (!examExists)
            {
                throw new NotFoundException("Mülakat sınavı bulunamadı");
            }

            InterviewAnswerEntity answer = new();
            answer.CreateAnswer(request.UserId, request.InterviewExamId, request.AnswerDate);

            await answerRepository.AddAsync(answer);
            await answerRepository.SaveChangesAsync();
        }
    }
}


