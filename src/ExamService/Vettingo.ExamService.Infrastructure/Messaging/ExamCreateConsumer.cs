using DotNetCore.CAP;
using Vettingo.ExamService.Infrastructure.Messages;

namespace Vettingo.ExamService.Infrastructure.Messaging;

public sealed class ExamCreateConsumer : ICapSubscribe
{
    [CapSubscribe("vettingo.exam.job-stage-exam.create.requested.v1", Group = "vettingo.exam-service")]
    public Task HandleAsync(ExamCreateMessage message)
    {
        return Task.CompletedTask;
    }
}
