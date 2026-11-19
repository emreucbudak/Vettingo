using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.UnitOfWork;
using DeleteExam;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam
{
    public class DeleteExamCommandHandler(IExamUnitOfWork unitOfWork, ILogger<DeleteExamCommandHandler> logger, ExamDeletesService.ExamDeletesServiceClient client) : IRequestHandler<DeleteExamCommandRequest>
    {
        public async Task Handle(DeleteExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteExamCommandHandler));
            var exam = await unitOfWork.ExamRepository.GetExamByIdAndCompanyIdAsync(request.ExamId, request.CompanyId, cancellationToken);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }
            var deleteExamRequest = new DeleteExamRequest();
            deleteExamRequest.ExamId = exam.Id.ToString();
            var examInUse = await client.DeleteExamAsync(deleteExamRequest);
            if (examInUse.Success)
            {
                throw new Exception("Bu Sınav Kullanımda!");
            }
            exam.UpdateExamActivate();
            unitOfWork.ExamRepository.Update(exam);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}



