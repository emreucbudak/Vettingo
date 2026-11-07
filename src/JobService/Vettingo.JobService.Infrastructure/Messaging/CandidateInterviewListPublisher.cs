using DotNetCore.CAP;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class CandidateInterviewListPublisher
{
    private readonly ICapPublisher _capPublisher;

    public CandidateInterviewListPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }
}
