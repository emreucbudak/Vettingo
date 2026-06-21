using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam
{
    public class DeleteExamCommandHandler(IExamRepository examRepository, ILogger<DeleteExamCommandHandler> logger) : IRequestHandler<DeleteExamCommandRequest>
    {
        public async Task Handle(DeleteExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteExamCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            examRepository.DeleteExam(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}



