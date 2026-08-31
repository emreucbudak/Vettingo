using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;
using Vettingo.AuthService.Application.Payment;

namespace Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

public sealed class ConfirmSubscriptionPaymentCommandHandler(
    ISubscriptionPaymentGateway paymentGateway,
    IDistributedCache cache,
    IMediator mediator,
    ILogger<ConfirmSubscriptionPaymentCommandHandler> logger)
    : IRequestHandler<
        ConfirmSubscriptionPaymentCommandRequest,
        ConfirmSubscriptionPaymentCommandResponse>
{
    private const string Currency = "usd";

    public async Task<ConfirmSubscriptionPaymentCommandResponse> Handle(
        ConfirmSubscriptionPaymentCommandRequest request,
        CancellationToken cancellationToken)
    {
        string accountType = request.AccountType.Trim().ToLowerInvariant();
        string planCode = request.PlanId.Trim().ToLowerInvariant();
        string billingPeriod = request.BillingPeriod.Trim().ToLowerInvariant();
        long amountInMinorUnits = GetAmountInMinorUnits(
            accountType,
            planCode,
            billingPeriod);

        if (amountInMinorUnits == 0)
        {
            throw new BadRequestException(
                "Ücretsiz planlar ödeme endpoint'i yerine ücretsiz aktivasyon akışını kullanmalıdır.");
        }

        string paymentCacheKey =
            $"subscription-payment:{request.RegistrationToken:D}";
        string? paymentIntentId = request.PaymentIntentId;

        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            paymentIntentId = await cache.GetStringAsync(
                paymentCacheKey,
                cancellationToken);
        }

        SubscriptionPaymentResult payment;

        if (!string.IsNullOrWhiteSpace(paymentIntentId))
        {
            payment = await paymentGateway.GetAsync(
                paymentIntentId,
                cancellationToken);
        }
        else
        {
            payment = await paymentGateway.ConfirmAsync(
                new SubscriptionPaymentRequest
                {
                    ConfirmationTokenId = request.ConfirmationTokenId!,
                    AccountType = accountType,
                    PlanCode = planCode,
                    BillingPeriod = billingPeriod,
                    RegistrationToken = request.RegistrationToken,
                    AmountInMinorUnits = amountInMinorUnits,
                    Currency = Currency
                },
                cancellationToken);

            if (payment.Status != SubscriptionPaymentStatus.Failed)
            {
                await cache.SetStringAsync(
                    paymentCacheKey,
                    payment.PaymentIntentId,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    },
                    cancellationToken);
            }
        }

        EnsurePaymentMatchesRequest(
            payment,
            accountType,
            planCode,
            billingPeriod,
            request.RegistrationToken,
            amountInMinorUnits);

        if (payment.RegistrationCompleted)
        {
            return Completed(payment.PaymentIntentId);
        }

        if (payment.Status == SubscriptionPaymentStatus.RequiresAction)
        {
            return new ConfirmSubscriptionPaymentCommandResponse
            {
                Completed = false,
                ClientSecret = payment.ClientSecret,
                Message = "Ödemeyi tamamlamak için banka doğrulaması gerekiyor.",
                PaymentIntentId = payment.PaymentIntentId,
                Status = "requires_action"
            };
        }

        if (payment.Status == SubscriptionPaymentStatus.Processing)
        {
            return new ConfirmSubscriptionPaymentCommandResponse
            {
                Completed = false,
                Message = "Ödemeniz Stripe tarafından işleniyor. Durumu biraz sonra tekrar kontrol edin.",
                PaymentIntentId = payment.PaymentIntentId,
                Status = "processing"
            };
        }

        if (payment.Status != SubscriptionPaymentStatus.Succeeded)
        {
            throw new BadRequestException(
                payment.FailureMessage ?? "Ödeme Stripe tarafından onaylanmadı.");
        }

        try
        {
            if (accountType == "candidate")
            {
                await mediator.Send(
                    new CandidateRegisterCommandRequest
                    {
                        Token = request.RegistrationToken
                    },
                    cancellationToken);
            }
            else
            {
                await mediator.Send(
                    new EmployerRegisterCommandRequest
                    {
                        Token = request.RegistrationToken,
                        PlanCode = planCode,
                        BillingPeriod = billingPeriod
                    },
                    cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Stripe ödemesi {PaymentIntentId} başarılı oldu ancak {AccountType} kaydı tamamlanamadı",
                payment.PaymentIntentId,
                accountType);

            return new ConfirmSubscriptionPaymentCommandResponse
            {
                Completed = false,
                Message = "Ödeme alındı ancak hesabınız etkinleştirilemedi. Kartınızdan yeniden çekim yapılmadan tekrar deneyebilirsiniz.",
                PaymentIntentId = payment.PaymentIntentId,
                Status = "registration_failed"
            };
        }

        try
        {
            await paymentGateway.MarkRegistrationCompletedAsync(
                payment.PaymentIntentId,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Tamamlanan kayıt bilgisi Stripe PaymentIntent {PaymentIntentId} metadata'sına yazılamadı",
                payment.PaymentIntentId);
        }

        return Completed(payment.PaymentIntentId);
    }

    private static ConfirmSubscriptionPaymentCommandResponse Completed(
        string paymentIntentId)
    {
        return new ConfirmSubscriptionPaymentCommandResponse
        {
            Completed = true,
            Message = "Ödeme ve hesap aktivasyonu tamamlandı.",
            PaymentIntentId = paymentIntentId,
            Status = "succeeded"
        };
    }

    private static void EnsurePaymentMatchesRequest(
        SubscriptionPaymentResult payment,
        string accountType,
        string planCode,
        string billingPeriod,
        Guid registrationToken,
        long amountInMinorUnits)
    {
        bool matches =
            string.Equals(payment.AccountType, accountType, StringComparison.Ordinal) &&
            string.Equals(payment.PlanCode, planCode, StringComparison.Ordinal) &&
            string.Equals(payment.BillingPeriod, billingPeriod, StringComparison.Ordinal) &&
            payment.RegistrationToken == registrationToken &&
            payment.AmountInMinorUnits == amountInMinorUnits &&
            string.Equals(payment.Currency, Currency, StringComparison.OrdinalIgnoreCase);

        if (!matches)
        {
            throw new BadRequestException(
                "Ödeme bilgileri seçilen plan veya kayıt oturumu ile eşleşmiyor.");
        }
    }

    private static long GetAmountInMinorUnits(
        string accountType,
        string planCode,
        string billingPeriod)
    {
        return (accountType, planCode, billingPeriod) switch
        {
            ("candidate", "basic", _) => 0,
            ("candidate", "pro", "monthly") => 999,
            ("candidate", "pro", "annual") => 5_988,
            ("candidate", "ultra", "monthly") => 1_999,
            ("candidate", "ultra", "annual") => 11_988,
            ("employer", "basic", _) => 0,
            ("employer", "pro", "monthly") => 2_999,
            ("employer", "pro", "annual") => 17_988,
            ("employer", "ultra", "monthly") => 4_599,
            ("employer", "ultra", "annual") => 27_588,
            _ => throw new BadRequestException(
                "Hesap türü, plan veya faturalandırma dönemi geçersiz.")
        };
    }
}
