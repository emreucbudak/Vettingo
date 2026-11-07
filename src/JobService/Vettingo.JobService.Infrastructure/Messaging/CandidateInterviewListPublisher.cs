using DotNetCore.CAP;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class CandidateInterviewListPublisher : ICandidateInterviewListPublisher
{
    private readonly ICapPublisher _capPublisher;

    public CandidateInterviewListPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }
}
