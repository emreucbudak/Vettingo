using DotNetCore.CAP;
using Vettingo.InterviewService.Infrastructure.Messages;

namespace Vettingo.InterviewService.Infrastructure.Messaging;

public sealed class InterviewCreateConsumer : ICapSubscribe
{
    [CapSubscribe("vettingo.interview.job-interview.create.requested.v1", Group = "vettingo.interview-service")]
    public Task HandleAsync(InterviewCreateMessage message)
    {
        return Task.CompletedTask;
    }
}
