using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class SubscriptionRegistrationRequestedConsumer(
    IMediator mediator,
    ILogger<SubscriptionRegistrationRequestedConsumer> logger)
    : ICapSubscribe
{
    [CapSubscribe(
        SubscriptionRegistrationRequestedMessage.TopicName,
        Group = "vettingo.auth-service")]
    public async Task HandleAsync(
        SubscriptionRegistrationRequestedMessage message,
        CancellationToken cancellationToken)
    {
        string accountType = message.AccountType.Trim().ToLowerInvariant();

        logger.LogInformation(
            "Subscription kayıt eventi işleniyor. AccountType: {AccountType}, SubscriberId: {SubscriberId}",
            accountType,
            message.SubscriberId);

        switch (accountType)
        {
            case "candidate":
                await mediator.Send(
                    new CandidateRegisterCommandRequest
                    {
                        Token = message.RegistrationToken,
                        SubscriberId = message.SubscriberId
                    },
                    cancellationToken);
                break;

            case "employer":
                await mediator.Send(
                    new EmployerRegisterCommandRequest
                    {
                        Token = message.RegistrationToken,
                        SubscriberId = message.SubscriberId
                    },
                    cancellationToken);
                break;

            default:
                throw new InvalidOperationException(
                    $"Desteklenmeyen hesap türü: {message.AccountType}");
        }
    }
}
