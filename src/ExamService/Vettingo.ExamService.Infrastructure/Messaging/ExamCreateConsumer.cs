using DotNetCore.CAP;
using FlashMediator;
using Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Command.CreateJobStageExam;
using Vettingo.ExamService.Infrastructure.Messages;

namespace Vettingo.ExamService.Infrastructure.Messaging;

public sealed class ExamCreateConsumer(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe("vettingo.exam.job-stage-exam.create.requested.v1", Group = "vettingo.exam-service")]
    public async Task HandleAsync(ExamCreateMessage message, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateJobStageExamCommandRequest
        {
            JobId = message.JobId,
            StepId = message.StepId,
            ExamId = message.ExamId,
            StartedDate = message.StartedDate,
            EndDate = message.EndDate,
            ExamCandidateList = message.CandidateIds
        }, cancellationToken);
    }
}
