using FlashMediator;
using FluentAssertions;
using NSubstitute;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

namespace Vettingo.AuthService.UnitTests;

public sealed class ActivateFreeSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRouteCandidateRegistrationThroughMediator()
    {
        Guid registrationToken = Guid.NewGuid();
        IMediator mediator = CreateMediator();
        ActivateFreeSubscriptionCommandHandler handler = new(mediator);

        ActivateFreeSubscriptionCommandResponse response = await handler.Handle(
            new ActivateFreeSubscriptionCommandRequest
            {
                AccountType = "Candidate",
                BillingPeriod = "monthly",
                PlanId = "Basic",
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        response.Completed.Should().BeTrue();
        await mediator.Received(1).Send(
            Arg.Is<IRequest>(command =>
                command.GetType() == typeof(CandidateRegisterCommandRequest) &&
                ((CandidateRegisterCommandRequest)command).Token == registrationToken),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRouteEmployerRegistrationWithFreePlanDetails()
    {
        Guid registrationToken = Guid.NewGuid();
        IMediator mediator = CreateMediator();
        ActivateFreeSubscriptionCommandHandler handler = new(mediator);

        ActivateFreeSubscriptionCommandResponse response = await handler.Handle(
            new ActivateFreeSubscriptionCommandRequest
            {
                AccountType = "employer",
                BillingPeriod = "Annual",
                PlanId = "basic",
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        response.Completed.Should().BeTrue();
        await mediator.Received(1).Send(
            Arg.Is<IRequest>(command =>
                command.GetType() == typeof(EmployerRegisterCommandRequest) &&
                ((EmployerRegisterCommandRequest)command).Token == registrationToken &&
                ((EmployerRegisterCommandRequest)command).PlanCode == "basic" &&
                ((EmployerRegisterCommandRequest)command).BillingPeriod == "annual"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRejectPaidPlanWithoutDispatchingRegistration()
    {
        IMediator mediator = CreateMediator();
        ActivateFreeSubscriptionCommandHandler handler = new(mediator);

        Func<Task> action = () => handler.Handle(
            new ActivateFreeSubscriptionCommandRequest
            {
                AccountType = "candidate",
                BillingPeriod = "monthly",
                PlanId = "pro",
                RegistrationToken = Guid.NewGuid()
            },
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*Basic*");
        await mediator.DidNotReceiveWithAnyArgs().Send(
            default(IRequest)!,
            default);
    }

    private static IMediator CreateMediator()
    {
        IMediator mediator = Substitute.For<IMediator>();
        mediator
            .Send(Arg.Any<IRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        return mediator;
    }
}
