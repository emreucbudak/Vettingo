using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.UnitOfWork;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam
{
    public class DeleteExamCommandHandler(IExamUnitOfWork unitOfWork, ILogger<DeleteExamCommandHandler> logger) : IRequestHandler<DeleteExamCommandRequest>
    {
        public async Task Handle(DeleteExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteExamCommandHandler));
            var exam = await unitOfWork.ExamRepository.GetExamByIdAndCompanyIdAsync(request.ExamId, request.CompanyId, cancellationToken);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            unitOfWork.ExamRepository.Delete(exam);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}



