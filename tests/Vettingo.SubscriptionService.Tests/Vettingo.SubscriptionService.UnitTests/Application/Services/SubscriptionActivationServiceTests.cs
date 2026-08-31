using FlashMediator;
using FluentAssertions;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;
using Vettingo.SubscriptionService.Application.Features.CQRS.CompanySubscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Application.Messaging;
using Vettingo.SubscriptionService.Application.Services;

namespace Vettingo.SubscriptionService.UnitTests.Application.Services;

public sealed class SubscriptionActivationServiceTests
{
    [Theory]
    [InlineData("candidate")]
    [InlineData("employer")]
    public async Task ActivateAsync_ShouldCreateSubscriptionBeforePublishingAuthRegistration(
        string accountType)
    {
        Guid subscriberId = Guid.NewGuid();
        Guid registrationToken = Guid.NewGuid();
        IMediator mediator = Substitute.For<IMediator>();
        ISubscriptionRegistrationPublisher publisher =
            Substitute.For<ISubscriptionRegistrationPublisher>();
        List<string> callOrder = [];
        mediator
            .Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callOrder.Add("subscription");
                return Guid.NewGuid();
            });
        publisher
            .PublishRegistrationRequestedAsync(
                Arg.Any<SubscriptionRegistrationRequestedEvent>(),
                Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callOrder.Add("auth-event");
                return Task.CompletedTask;
            });
        SubscriptionActivationService service = new(mediator, publisher);

        await service.ActivateAsync(
            accountType,
            subscriberId,
            42,
            "annual",
            registrationToken,
            CancellationToken.None);

        callOrder.Should().Equal("subscription", "auth-event");

        if (accountType == "candidate")
        {
            await mediator.Received(1).Send(
                Arg.Is<IRequest<Guid>>(request =>
                    request.GetType() == typeof(CreateCandidateSubscriptionCommandRequest) &&
                    ((CreateCandidateSubscriptionCommandRequest)request).CandidateId == subscriberId &&
                    ((CreateCandidateSubscriptionCommandRequest)request).PlanId == 42),
                Arg.Any<CancellationToken>());
        }
        else
        {
            await mediator.Received(1).Send(
                Arg.Is<IRequest<Guid>>(request =>
                    request.GetType() == typeof(CreateCompanySubscriptionCommandRequest) &&
                    ((CreateCompanySubscriptionCommandRequest)request).CompanyId == subscriberId &&
                    ((CreateCompanySubscriptionCommandRequest)request).PlanId == 42),
                Arg.Any<CancellationToken>());
        }

        await publisher.Received(1).PublishRegistrationRequestedAsync(
            Arg.Is<SubscriptionRegistrationRequestedEvent>(message =>
                message.AccountType == accountType &&
                message.RegistrationToken == registrationToken &&
                message.SubscriberId == subscriberId),
            Arg.Any<CancellationToken>());
    }
}
