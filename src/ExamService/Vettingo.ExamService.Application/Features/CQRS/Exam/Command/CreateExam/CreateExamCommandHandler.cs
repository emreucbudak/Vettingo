using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam
{
    public class CreateExamCommandHandler(IExamRepository examRepository) : IRequestHandler<CreateExamCommandRequest>
    {
        public async Task Handle(CreateExamCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Exam exam = new();
            exam.CreateExam(request.Title, request.Subject, request.Description, request.DurationMinutes, request.PassingScore, request.OwnerType, request.CompanyId, request.JobId);

            await examRepository.AddExamAsync(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}
