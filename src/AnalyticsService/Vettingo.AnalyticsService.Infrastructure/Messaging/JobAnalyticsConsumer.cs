using DotNetCore.CAP;
using FlashMediator;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.UpdateCompanyJobPostingCount;

namespace Vettingo.AnalyticsService.Infrastructure.Messaging;

public sealed class JobAnalyticsConsumer(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe(JobAnalyticsMessage.TopicName, Group = "vettingo.analytics-service")]
    public Task HandleAsync(JobAnalyticsMessage message, CancellationToken cancellationToken)
    {
        return mediator.Send(
            new UpdateCompanyJobPostingCountCommandRequest
            {
                CompanyId = message.CompanyId
            },
            cancellationToken);
    }
}
