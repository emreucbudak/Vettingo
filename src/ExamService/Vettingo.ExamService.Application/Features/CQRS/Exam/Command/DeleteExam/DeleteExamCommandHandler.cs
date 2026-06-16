using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam
{
    public class DeleteExamCommandHandler(IExamRepository examRepository) : IRequestHandler<DeleteExamCommandRequest>
    {
        public async Task Handle(DeleteExamCommandRequest request, CancellationToken cancellationToken)
        {
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new Exception("Exam not found");
            }

            examRepository.DeleteExam(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}
