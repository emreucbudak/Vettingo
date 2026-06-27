using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.DeleteInterviewExam
{
    public class DeleteInterviewExamCommandHandler(IRepository<InterviewExamEntity> examRepository, ILogger<DeleteInterviewExamCommandHandler> logger) : IRequestHandler<DeleteInterviewExamCommandRequest>
    {
        public async Task Handle(DeleteInterviewExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteInterviewExamCommandHandler));

            InterviewExamEntity exam = await examRepository.GetByIdAsync(request.InterviewExamId)
                ?? throw new NotFoundException("Mülakat sınavı bulunamadı");

            examRepository.Delete(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}


