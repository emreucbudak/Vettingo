using DotNetCore.CAP;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class CandidateExamListPublisher
{
    private readonly ICapPublisher _capPublisher;

    public CandidateExamListPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }
}
