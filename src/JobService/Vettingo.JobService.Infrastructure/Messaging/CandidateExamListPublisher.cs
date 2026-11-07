using DotNetCore.CAP;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class CandidateExamListPublisher : ICandidateExamListPublisher
{
    private readonly ICapPublisher _capPublisher;

    public CandidateExamListPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }
}
