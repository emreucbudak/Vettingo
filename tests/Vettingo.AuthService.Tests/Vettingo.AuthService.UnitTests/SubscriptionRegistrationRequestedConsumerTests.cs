using FlashMediator;
using NSubstitute;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;
using Vettingo.AuthService.Infrastructure.Messaging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Vettingo.AuthService.UnitTests;

public sealed class SubscriptionRegistrationRequestedConsumerTests
{
    [Theory]
    [InlineData("candidate")]
    [InlineData("employer")]
    public async Task HandleAsync_ShouldRouteRegistrationToExpectedAuthCommand(
        string accountType)
    {
        Guid registrationToken = Guid.NewGuid();
        Guid subscriberId = Guid.NewGuid();
        IMediator mediator = Substitute.For<IMediator>();
        mediator
            .Send(Arg.Any<IRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        SubscriptionRegistrationRequestedConsumer consumer = new(
            mediator,
            NullLogger<SubscriptionRegistrationRequestedConsumer>.Instance);

        await consumer.HandleAsync(
            new SubscriptionRegistrationRequestedMessage
            {
                AccountType = accountType,
                RegistrationToken = registrationToken,
                SubscriberId = subscriberId
            },
            CancellationToken.None);

        if (accountType == "candidate")
        {
            await mediator.Received(1).Send(
                Arg.Is<IRequest>(request =>
                    request.GetType() == typeof(CandidateRegisterCommandRequest) &&
                    ((CandidateRegisterCommandRequest)request).Token == registrationToken &&
                    ((CandidateRegisterCommandRequest)request).SubscriberId == subscriberId),
                Arg.Any<CancellationToken>());
        }
        else
        {
            await mediator.Received(1).Send(
                Arg.Is<IRequest>(request =>
                    request.GetType() == typeof(EmployerRegisterCommandRequest) &&
                    ((EmployerRegisterCommandRequest)request).Token == registrationToken &&
                    ((EmployerRegisterCommandRequest)request).SubscriberId == subscriberId),
                Arg.Any<CancellationToken>());
        }
    }
}
