using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Payment;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Application.Services;
using Vettingo.SubscriptionService.Domain.Enums;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

public sealed class ConfirmSubscriptionPaymentCommandHandler(
    ISubscriptionPaymentGateway paymentGateway,
    IPlanRepository planRepository,
    IDistributedCache cache,
    ISubscriptionActivationService activationService,
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
        string billingPeriod = request.BillingPeriod.Trim().ToLowerInvariant();
        PlanEntity plan = await planRepository.GetPlanByIdAsync(
            request.PlanId,
            cancellationToken)
            ?? throw new NotFoundException(
                $"{request.PlanId} kimlikli plan bulunamadı.");
        PlanType expectedPlanType = accountType switch
        {
            "candidate" => PlanType.Candidate,
            "employer" => PlanType.Employer,
            _ => throw new BadRequestException("Geçersiz hesap türü.")
        };

        if (plan.PlanType != expectedPlanType)
        {
            throw new BadRequestException(
                "Seçilen plan hesap türüyle eşleşmiyor.");
        }

        string paymentCacheKey =
            $"subscription-payment:{accountType}:{request.RegistrationToken:D}:{plan.Id}:{billingPeriod}";
        string? paymentIntentId = request.PaymentIntentId;

        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            paymentIntentId = await cache.GetStringAsync(
                paymentCacheKey,
                cancellationToken);
        }

        SubscriptionPaymentResult payment;
        long amountInMinorUnits;

        if (!string.IsNullOrWhiteSpace(paymentIntentId))
        {
            amountInMinorUnits = (long)request.Amount * 100;
            payment = await paymentGateway.GetAsync(
                paymentIntentId,
                cancellationToken);
        }
        else
        {
            long expectedAmount = (long)plan.Price *
                (billingPeriod == "annual" ? 12 : 1);

            if (request.Amount != expectedAmount)
            {
                throw new BadRequestException(
                    "Gönderilen tutar seçilen planın güncel fiyatıyla eşleşmiyor.");
            }

            if (expectedAmount == 0)
            {
                throw new BadRequestException(
                    "Ücretsiz planlar ödeme endpoint'i yerine ücretsiz aktivasyon akışını kullanmalıdır.");
            }

            amountInMinorUnits = expectedAmount * 100;
            payment = await paymentGateway.ConfirmAsync(
                new SubscriptionPaymentRequest
                {
                    ConfirmationTokenId = request.ConfirmationTokenId!,
                    AccountType = accountType,
                    PlanId = plan.Id,
                    PlanName = plan.PlanName,
                    BillingPeriod = billingPeriod,
                    RegistrationToken = request.RegistrationToken,
                    SubscriberId = Guid.CreateVersion7(),
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
            plan.Id,
            billingPeriod,
            request.RegistrationToken,
            amountInMinorUnits);

        if (payment.AuthRegistrationRequested)
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
            await activationService.ActivateAsync(
                accountType,
                payment.SubscriberId,
                plan.Id,
                billingPeriod,
                request.RegistrationToken,
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Stripe ödemesi {PaymentIntentId} başarılı oldu ancak abonelik ve Auth kayıt eventi tamamlanamadı",
                payment.PaymentIntentId);

            return new ConfirmSubscriptionPaymentCommandResponse
            {
                Completed = false,
                Message = "Ödeme alındı ancak abonelik etkinleştirilemedi. Kartınızdan yeniden çekim yapılmadan tekrar deneyebilirsiniz.",
                PaymentIntentId = payment.PaymentIntentId,
                Status = "activation_failed"
            };
        }

        try
        {
            await paymentGateway.MarkAuthRegistrationRequestedAsync(
                payment.PaymentIntentId,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Auth kayıt eventi yayınlanan Stripe PaymentIntent {PaymentIntentId} metadata'sı güncellenemedi",
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
            Message = "Ödeme ve abonelik aktivasyonu tamamlandı. Hesap kaydı işleme alındı.",
            PaymentIntentId = paymentIntentId,
            Status = "succeeded"
        };
    }

    private static void EnsurePaymentMatchesRequest(
        SubscriptionPaymentResult payment,
        string accountType,
        int planId,
        string billingPeriod,
        Guid registrationToken,
        long amountInMinorUnits)
    {
        bool matches =
            string.Equals(payment.AccountType, accountType, StringComparison.Ordinal) &&
            payment.PlanId == planId &&
            string.Equals(payment.BillingPeriod, billingPeriod, StringComparison.Ordinal) &&
            payment.RegistrationToken == registrationToken &&
            payment.SubscriberId != Guid.Empty &&
            payment.AmountInMinorUnits == amountInMinorUnits &&
            string.Equals(payment.Currency, Currency, StringComparison.OrdinalIgnoreCase);

        if (!matches)
        {
            throw new BadRequestException(
                "Ödeme bilgileri seçilen plan veya kayıt oturumu ile eşleşmiyor.");
        }
    }
}
