using FlashMediator;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;
using Vettingo.AuthService.Application.Payment;

namespace Vettingo.AuthService.UnitTests;

public sealed class ConfirmSubscriptionPaymentCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRegisterCandidateAfterSuccessfulStripePayment()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        IMediator mediator = Substitute.For<IMediator>();
        mediator
            .Send(Arg.Any<IRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        paymentGateway
            .ConfirmAsync(
                Arg.Any<SubscriptionPaymentRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                SubscriptionPaymentRequest paymentRequest = call.Arg<SubscriptionPaymentRequest>();
                return SuccessfulPayment(paymentRequest, "pi_candidate");
            });

        ConfirmSubscriptionPaymentCommandHandler handler = CreateHandler(
            paymentGateway,
            mediator);

        ConfirmSubscriptionPaymentCommandResponse response = await handler.Handle(
            new ConfirmSubscriptionPaymentCommandRequest
            {
                AccountType = "candidate",
                BillingPeriod = "monthly",
                ConfirmationTokenId = "ct_candidate",
                PlanId = "pro",
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        response.Completed.Should().BeTrue();
        response.PaymentIntentId.Should().Be("pi_candidate");
        await paymentGateway.Received(1).ConfirmAsync(
            Arg.Is<SubscriptionPaymentRequest>(payment =>
                payment.AccountType == "candidate" &&
                payment.PlanCode == "pro" &&
                payment.BillingPeriod == "monthly" &&
                payment.AmountInMinorUnits == 999 &&
                payment.RegistrationToken == registrationToken),
            Arg.Any<CancellationToken>());
        await mediator.Received(1).Send(
            Arg.Is<IRequest>(command =>
                command.GetType() == typeof(CandidateRegisterCommandRequest) &&
                ((CandidateRegisterCommandRequest)command).Token == registrationToken &&
                ((CandidateRegisterCommandRequest)command).PlanCode == "pro" &&
                ((CandidateRegisterCommandRequest)command).BillingPeriod == "monthly"),
            Arg.Any<CancellationToken>());
        await paymentGateway.Received(1).MarkRegistrationCompletedAsync(
            "pi_candidate",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRegisterEmployerWhenPaidIntentIsFinalized()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        IMediator mediator = Substitute.For<IMediator>();
        mediator
            .Send(Arg.Any<IRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        paymentGateway
            .GetAsync("pi_employer", Arg.Any<CancellationToken>())
            .Returns(new SubscriptionPaymentResult
            {
                AccountType = "employer",
                AmountInMinorUnits = 17_988,
                BillingPeriod = "annual",
                Currency = "usd",
                PaymentIntentId = "pi_employer",
                PlanCode = "pro",
                RegistrationToken = registrationToken,
                Status = SubscriptionPaymentStatus.Succeeded
            });

        ConfirmSubscriptionPaymentCommandHandler handler = CreateHandler(
            paymentGateway,
            mediator);

        ConfirmSubscriptionPaymentCommandResponse response = await handler.Handle(
            new ConfirmSubscriptionPaymentCommandRequest
            {
                AccountType = "employer",
                BillingPeriod = "annual",
                PaymentIntentId = "pi_employer",
                PlanId = "pro",
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        response.Completed.Should().BeTrue();
        await mediator.Received(1).Send(
            Arg.Is<IRequest>(command =>
                command.GetType() == typeof(EmployerRegisterCommandRequest) &&
                ((EmployerRegisterCommandRequest)command).Token == registrationToken &&
                ((EmployerRegisterCommandRequest)command).PlanCode == "pro" &&
                ((EmployerRegisterCommandRequest)command).BillingPeriod == "annual"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNotRegisterBeforeStripeActionSucceeds()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        IMediator mediator = Substitute.For<IMediator>();
        paymentGateway
            .ConfirmAsync(
                Arg.Any<SubscriptionPaymentRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                SubscriptionPaymentRequest paymentRequest = call.Arg<SubscriptionPaymentRequest>();
                return new SubscriptionPaymentResult
                {
                    AccountType = paymentRequest.AccountType,
                    AmountInMinorUnits = paymentRequest.AmountInMinorUnits,
                    BillingPeriod = paymentRequest.BillingPeriod,
                    ClientSecret = "pi_secret",
                    Currency = paymentRequest.Currency,
                    PaymentIntentId = "pi_action",
                    PlanCode = paymentRequest.PlanCode,
                    RegistrationToken = paymentRequest.RegistrationToken,
                    Status = SubscriptionPaymentStatus.RequiresAction
                };
            });

        ConfirmSubscriptionPaymentCommandResponse response = await CreateHandler(
                paymentGateway,
                mediator)
            .Handle(
                new ConfirmSubscriptionPaymentCommandRequest
                {
                    AccountType = "candidate",
                    BillingPeriod = "monthly",
                    ConfirmationTokenId = "ct_action",
                    PlanId = "ultra",
                    RegistrationToken = registrationToken
                },
                CancellationToken.None);

        response.Completed.Should().BeFalse();
        response.Status.Should().Be("requires_action");
        response.ClientSecret.Should().Be("pi_secret");
        await mediator.DidNotReceiveWithAnyArgs().Send(
            default(IRequest)!,
            default);
    }

    [Fact]
    public async Task Handle_ShouldReuseCachedPaymentIntentInsteadOfChargingAgain()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        IMediator mediator = Substitute.For<IMediator>();
        cache
            .GetAsync(
                $"subscription-payment:{registrationToken:D}",
                Arg.Any<CancellationToken>())
            .Returns(Encoding.UTF8.GetBytes("pi_cached"));
        paymentGateway
            .GetAsync("pi_cached", Arg.Any<CancellationToken>())
            .Returns(new SubscriptionPaymentResult
            {
                AccountType = "candidate",
                AmountInMinorUnits = 999,
                BillingPeriod = "monthly",
                Currency = "usd",
                PaymentIntentId = "pi_cached",
                PlanCode = "pro",
                RegistrationCompleted = true,
                RegistrationToken = registrationToken,
                Status = SubscriptionPaymentStatus.Succeeded
            });

        ConfirmSubscriptionPaymentCommandResponse response = await CreateHandler(
                paymentGateway,
                mediator,
                cache)
            .Handle(
                new ConfirmSubscriptionPaymentCommandRequest
                {
                    AccountType = "candidate",
                    BillingPeriod = "monthly",
                    ConfirmationTokenId = "ct_retry",
                    PlanId = "pro",
                    RegistrationToken = registrationToken
                },
                CancellationToken.None);

        response.Completed.Should().BeTrue();
        response.PaymentIntentId.Should().Be("pi_cached");
        await paymentGateway.DidNotReceiveWithAnyArgs().ConfirmAsync(
            default!,
            default);
        await mediator.DidNotReceiveWithAnyArgs().Send(
            default(IRequest)!,
            default);
    }

    private static ConfirmSubscriptionPaymentCommandHandler CreateHandler(
        ISubscriptionPaymentGateway paymentGateway,
        IMediator mediator,
        IDistributedCache? cache = null)
    {
        return new ConfirmSubscriptionPaymentCommandHandler(
            paymentGateway,
            cache ?? Substitute.For<IDistributedCache>(),
            mediator,
            NullLogger<ConfirmSubscriptionPaymentCommandHandler>.Instance);
    }

    private static SubscriptionPaymentResult SuccessfulPayment(
        SubscriptionPaymentRequest request,
        string paymentIntentId)
    {
        return new SubscriptionPaymentResult
        {
            AccountType = request.AccountType,
            AmountInMinorUnits = request.AmountInMinorUnits,
            BillingPeriod = request.BillingPeriod,
            Currency = request.Currency,
            PaymentIntentId = paymentIntentId,
            PlanCode = request.PlanCode,
            RegistrationToken = request.RegistrationToken,
            Status = SubscriptionPaymentStatus.Succeeded
        };
    }
}
