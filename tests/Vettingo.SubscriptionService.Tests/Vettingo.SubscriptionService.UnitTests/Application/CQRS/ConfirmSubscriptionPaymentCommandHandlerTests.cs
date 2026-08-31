using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Reflection;
using System.Text;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;
using Vettingo.SubscriptionService.Application.Payment;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Application.Services;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.UnitTests.Application.CQRS;

public sealed class ConfirmSubscriptionPaymentCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldActivateCandidateAfterSuccessfulStripePayment()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        paymentGateway
            .ConfirmAsync(
                Arg.Any<SubscriptionPaymentRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                SubscriptionPaymentRequest paymentRequest =
                    call.Arg<SubscriptionPaymentRequest>();
                return SuccessfulPayment(paymentRequest, "pi_candidate");
            });

        ConfirmSubscriptionPaymentCommandHandler handler = CreateHandler(
            paymentGateway,
            activationService,
            CreatePlanRepository(101, 10, PlanType.Candidate));

        ConfirmSubscriptionPaymentCommandResponse response = await handler.Handle(
            new ConfirmSubscriptionPaymentCommandRequest
            {
                AccountType = "candidate",
                Amount = 10,
                BillingPeriod = "monthly",
                ConfirmationTokenId = "ct_candidate",
                PlanId = 101,
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        response.Completed.Should().BeTrue();
        response.PaymentIntentId.Should().Be("pi_candidate");
        await paymentGateway.Received(1).ConfirmAsync(
            Arg.Is<SubscriptionPaymentRequest>(payment =>
                payment.AccountType == "candidate" &&
                payment.PlanId == 101 &&
                payment.BillingPeriod == "monthly" &&
                payment.AmountInMinorUnits == 1_000 &&
                payment.RegistrationToken == registrationToken &&
                payment.SubscriberId != Guid.Empty),
            Arg.Any<CancellationToken>());
        await activationService.Received(1).ActivateAsync(
            "candidate",
            Arg.Is<Guid>(subscriberId => subscriberId != Guid.Empty),
            101,
            "monthly",
            registrationToken,
            Arg.Any<CancellationToken>());
        await paymentGateway.Received(1).MarkAuthRegistrationRequestedAsync(
            "pi_candidate",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldActivateEmployerWhenPaidIntentIsFinalized()
    {
        Guid registrationToken = Guid.NewGuid();
        Guid subscriberId = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        paymentGateway
            .GetAsync("pi_employer", Arg.Any<CancellationToken>())
            .Returns(new SubscriptionPaymentResult
            {
                AccountType = "employer",
                AmountInMinorUnits = 36_000,
                BillingPeriod = "annual",
                Currency = "usd",
                PaymentIntentId = "pi_employer",
                PlanId = 202,
                RegistrationToken = registrationToken,
                SubscriberId = subscriberId,
                Status = SubscriptionPaymentStatus.Succeeded
            });

        ConfirmSubscriptionPaymentCommandResponse response = await CreateHandler(
                paymentGateway,
                activationService,
                CreatePlanRepository(202, 30, PlanType.Employer))
            .Handle(
                new ConfirmSubscriptionPaymentCommandRequest
                {
                    AccountType = "employer",
                    Amount = 360,
                    BillingPeriod = "annual",
                    PaymentIntentId = "pi_employer",
                    PlanId = 202,
                    RegistrationToken = registrationToken
                },
                CancellationToken.None);

        response.Completed.Should().BeTrue();
        await activationService.Received(1).ActivateAsync(
            "employer",
            subscriberId,
            202,
            "annual",
            registrationToken,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNotActivateBeforeStripeActionSucceeds()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        paymentGateway
            .ConfirmAsync(
                Arg.Any<SubscriptionPaymentRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                SubscriptionPaymentRequest paymentRequest =
                    call.Arg<SubscriptionPaymentRequest>();
                return new SubscriptionPaymentResult
                {
                    AccountType = paymentRequest.AccountType,
                    AmountInMinorUnits = paymentRequest.AmountInMinorUnits,
                    BillingPeriod = paymentRequest.BillingPeriod,
                    ClientSecret = "pi_secret",
                    Currency = paymentRequest.Currency,
                    PaymentIntentId = "pi_action",
                    PlanId = paymentRequest.PlanId,
                    RegistrationToken = paymentRequest.RegistrationToken,
                    SubscriberId = paymentRequest.SubscriberId,
                    Status = SubscriptionPaymentStatus.RequiresAction
                };
            });

        ConfirmSubscriptionPaymentCommandResponse response = await CreateHandler(
                paymentGateway,
                activationService,
                CreatePlanRepository(303, 20, PlanType.Candidate))
            .Handle(
                new ConfirmSubscriptionPaymentCommandRequest
                {
                    AccountType = "candidate",
                    Amount = 20,
                    BillingPeriod = "monthly",
                    ConfirmationTokenId = "ct_action",
                    PlanId = 303,
                    RegistrationToken = registrationToken
                },
                CancellationToken.None);

        response.Completed.Should().BeFalse();
        response.Status.Should().Be("requires_action");
        response.ClientSecret.Should().Be("pi_secret");
        await activationService.DidNotReceiveWithAnyArgs().ActivateAsync(
            default!,
            default,
            default,
            default!,
            default,
            default);
    }

    [Fact]
    public async Task Handle_ShouldReuseCachedPaymentIntentWhenPlanPriceChanges()
    {
        Guid registrationToken = Guid.NewGuid();
        Guid subscriberId = Guid.NewGuid();
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        IDistributedCache cache = CreateCache();
        cache
            .GetAsync(
                $"subscription-payment:candidate:{registrationToken:D}:101:monthly",
                Arg.Any<CancellationToken>())
            .Returns(Encoding.UTF8.GetBytes("pi_cached"));
        paymentGateway
            .GetAsync("pi_cached", Arg.Any<CancellationToken>())
            .Returns(new SubscriptionPaymentResult
            {
                AccountType = "candidate",
                AmountInMinorUnits = 1_000,
                BillingPeriod = "monthly",
                Currency = "usd",
                PaymentIntentId = "pi_cached",
                PlanId = 101,
                AuthRegistrationRequested = true,
                RegistrationToken = registrationToken,
                SubscriberId = subscriberId,
                Status = SubscriptionPaymentStatus.Succeeded
            });

        ConfirmSubscriptionPaymentCommandResponse response = await CreateHandler(
                paymentGateway,
                activationService,
                CreatePlanRepository(101, 99, PlanType.Candidate),
                cache)
            .Handle(
                new ConfirmSubscriptionPaymentCommandRequest
                {
                    AccountType = "candidate",
                    Amount = 10,
                    BillingPeriod = "monthly",
                    ConfirmationTokenId = "ct_retry",
                    PlanId = 101,
                    RegistrationToken = registrationToken
                },
                CancellationToken.None);

        response.Completed.Should().BeTrue();
        response.PaymentIntentId.Should().Be("pi_cached");
        await paymentGateway.DidNotReceiveWithAnyArgs().ConfirmAsync(
            default!,
            default);
        await activationService.DidNotReceiveWithAnyArgs().ActivateAsync(
            default!,
            default,
            default,
            default!,
            default,
            default);
    }

    [Fact]
    public async Task Handle_ShouldRejectClientAmountThatDoesNotMatchPlanPrice()
    {
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        ConfirmSubscriptionPaymentCommandHandler handler = CreateHandler(
            paymentGateway,
            activationService,
            CreatePlanRepository(404, 25, PlanType.Candidate));

        Func<Task> action = () => handler.Handle(
            new ConfirmSubscriptionPaymentCommandRequest
            {
                AccountType = "candidate",
                Amount = 1,
                BillingPeriod = "monthly",
                ConfirmationTokenId = "ct_tampered",
                PlanId = 404,
                RegistrationToken = Guid.NewGuid()
            },
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*güncel fiyatıyla eşleşmiyor*");
        await paymentGateway.DidNotReceiveWithAnyArgs().ConfirmAsync(
            default!,
            default);
    }

    [Fact]
    public async Task Handle_ShouldRejectPlanThatDoesNotMatchAccountType()
    {
        ISubscriptionPaymentGateway paymentGateway =
            Substitute.For<ISubscriptionPaymentGateway>();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        ConfirmSubscriptionPaymentCommandHandler handler = CreateHandler(
            paymentGateway,
            activationService,
            CreatePlanRepository(505, 25, PlanType.Employer));

        Func<Task> action = () => handler.Handle(
            new ConfirmSubscriptionPaymentCommandRequest
            {
                AccountType = "candidate",
                Amount = 25,
                BillingPeriod = "monthly",
                ConfirmationTokenId = "ct_wrong_type",
                PlanId = 505,
                RegistrationToken = Guid.NewGuid()
            },
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*hesap türüyle eşleşmiyor*");
    }

    private static ConfirmSubscriptionPaymentCommandHandler CreateHandler(
        ISubscriptionPaymentGateway paymentGateway,
        ISubscriptionActivationService activationService,
        IPlanRepository planRepository,
        IDistributedCache? cache = null)
    {
        return new ConfirmSubscriptionPaymentCommandHandler(
            paymentGateway,
            planRepository,
            cache ?? CreateCache(),
            activationService,
            NullLogger<ConfirmSubscriptionPaymentCommandHandler>.Instance);
    }

    private static IDistributedCache CreateCache()
    {
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache
            .SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        return cache;
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
            PlanId = request.PlanId,
            RegistrationToken = request.RegistrationToken,
            SubscriberId = request.SubscriberId,
            Status = SubscriptionPaymentStatus.Succeeded
        };
    }

    private static IPlanRepository CreatePlanRepository(
        int planId,
        int price,
        PlanType planType)
    {
        Plan plan = new();
        plan.CreatePlan("Dynamic plan", price, planType);
        typeof(Plan)
            .GetProperty(nameof(Plan.Id), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(plan, planId);

        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        planRepository
            .GetPlanByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        return planRepository;
    }
}
