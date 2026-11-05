using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.JobExam.Command.CreateJobExam
{
    public class CreateJobExamCommandHandler(IExamRepository<Domain.Entities.JobExam> examRepository, ILogger<CreateJobExamCommandHandler> logger) : IRequestHandler<CreateJobExamCommandRequest>
    {
        public async Task Handle(CreateJobExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateJobExamCommandHandler));
            Domain.Entities.JobExam jobExam = new();
            jobExam.CreateJobExam(request.JobId, request.ExamId);

            await examRepository.AddAsync(jobExam);
            await examRepository.SaveChangesAsync();
        }
    }
}
