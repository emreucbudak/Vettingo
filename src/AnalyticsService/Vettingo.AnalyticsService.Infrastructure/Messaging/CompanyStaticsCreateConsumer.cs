using DotNetCore.CAP;
using FlashMediator;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Command.CreateCompanyStatics;
using Vettingo.AnalyticsService.Infrastructure.Messages;

namespace Vettingo.AnalyticsService.Infrastructure.Messaging;

public sealed class CompanyStaticsCreateConsumer(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe(CreateCompanyStaticsMessage.TopicName, Group = "vettingo.analytics-service")]
    public Task HandleAsync(CreateCompanyStaticsMessage message, CancellationToken cancellationToken)
    {
        return mediator.Send(
            new CreateCompanyStaticsCommandRequest
            {
                CompanyId = message.CompanyId
            },
            cancellationToken);
    }
}
