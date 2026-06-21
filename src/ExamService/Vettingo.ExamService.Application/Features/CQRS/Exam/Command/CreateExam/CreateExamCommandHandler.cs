using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam
{
    public class CreateExamCommandHandler(IExamRepository examRepository, ILogger<CreateExamCommandHandler> logger) : IRequestHandler<CreateExamCommandRequest>
    {
        public async Task Handle(CreateExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateExamCommandHandler));
            Domain.Entities.Exam exam = new();
            exam.CreateExam(request.Title, request.Subject, request.Description, request.DurationMinutes, request.PassingScore, request.OwnerType, request.CompanyId, request.JobId);

            await examRepository.AddExamAsync(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}


