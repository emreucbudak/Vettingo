using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Command.CreateJobStageExam
{
    public class CreateJobStageExamCommandHandler(IExamRepository<Domain.Entities.JobStageExam> examRepository, ILogger<CreateJobStageExamCommandHandler> logger) : IRequestHandler<CreateJobStageExamCommandRequest>
    {
        public async Task Handle(CreateJobStageExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateJobStageExamCommandHandler));
            Domain.Entities.JobStageExam jobStageExam = new();
            jobStageExam.CreateJobExam(request.JobId, request.ExamId);

            await examRepository.AddAsync(jobStageExam);
            await examRepository.SaveChangesAsync();
        }
    }
}
