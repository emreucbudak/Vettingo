using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam
{
    public class UpdateExamCommandHandler(IExamRepository examRepository, ILogger<UpdateExamCommandHandler> logger) : IRequestHandler<UpdateExamCommandRequest>
    {
        public async Task Handle(UpdateExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateExamCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            exam.UpdateExam(request.Title, request.Subject, request.Description, request.DurationMinutes, request.PassingScore, request.OwnerType, request.CompanyId, request.JobId, request.IsActive);
            examRepository.UpdateExam(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}



