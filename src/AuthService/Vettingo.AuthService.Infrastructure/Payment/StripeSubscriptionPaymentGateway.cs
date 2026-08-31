using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Payment;

namespace Vettingo.AuthService.Infrastructure.Payment;

public sealed class StripeSubscriptionPaymentGateway(
    IOptions<StripeOptions> options,
    ILogger<StripeSubscriptionPaymentGateway> logger)
    : ISubscriptionPaymentGateway
{
    private const string AccountTypeMetadataKey = "vettingo_account_type";
    private const string BillingPeriodMetadataKey = "vettingo_billing_period";
    private const string PlanCodeMetadataKey = "vettingo_plan_code";
    private const string RegistrationCompletedMetadataKey = "vettingo_registration_completed";
    private const string RegistrationTokenMetadataKey = "vettingo_registration_token";

    public async Task<SubscriptionPaymentResult> ConfirmAsync(
        SubscriptionPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaymentIntentService service = CreateService();
            PaymentIntent paymentIntent = await service.CreateAsync(
                new PaymentIntentCreateOptions
                {
                    Amount = request.AmountInMinorUnits,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                    Confirm = true,
                    ConfirmationToken = request.ConfirmationTokenId,
                    Currency = request.Currency,
                    Description = $"Vettingo {request.AccountType} {request.PlanCode} {request.BillingPeriod} planı",
                    Metadata = new Dictionary<string, string>
                    {
                        [AccountTypeMetadataKey] = request.AccountType,
                        [BillingPeriodMetadataKey] = request.BillingPeriod,
                        [PlanCodeMetadataKey] = request.PlanCode,
                        [RegistrationCompletedMetadataKey] = bool.FalseString,
                        [RegistrationTokenMetadataKey] = request.RegistrationToken.ToString("D")
                    },
                    SetupFutureUsage = "off_session",
                    UseStripeSdk = true
                },
                new RequestOptions
                {
                    IdempotencyKey = $"vettingo-payment-{request.ConfirmationTokenId}"
                },
                cancellationToken);

            return Map(paymentIntent);
        }
        catch (StripeException exception)
        {
            logger.LogWarning(
                exception,
                "Stripe ConfirmationToken {ConfirmationTokenId} ile ödeme onaylanamadı",
                request.ConfirmationTokenId);

            throw new BadRequestException(
                exception.StripeError?.Message ?? "Ödeme Stripe tarafından onaylanmadı.");
        }
    }

    public async Task<SubscriptionPaymentResult> GetAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaymentIntent paymentIntent = await CreateService().GetAsync(
                paymentIntentId,
                cancellationToken: cancellationToken);

            return Map(paymentIntent);
        }
        catch (StripeException exception)
        {
            logger.LogWarning(
                exception,
                "Stripe PaymentIntent {PaymentIntentId} okunamadı",
                paymentIntentId);

            throw new BadRequestException("Ödeme kaydı Stripe üzerinde doğrulanamadı.");
        }
    }

    public async Task MarkRegistrationCompletedAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        await CreateService().UpdateAsync(
            paymentIntentId,
            new PaymentIntentUpdateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [RegistrationCompletedMetadataKey] = bool.TrueString
                }
            },
            cancellationToken: cancellationToken);
    }

    private PaymentIntentService CreateService()
    {
        if (string.IsNullOrWhiteSpace(options.Value.SecretKey))
        {
            throw new BusinessException(
                "Stripe secret key yapılandırılmadığı için ödeme başlatılamadı.");
        }

        return new PaymentIntentService(new StripeClient(options.Value.SecretKey));
    }

    private static SubscriptionPaymentResult Map(PaymentIntent paymentIntent)
    {
        IReadOnlyDictionary<string, string> metadata = paymentIntent.Metadata;
        metadata.TryGetValue(AccountTypeMetadataKey, out string? accountType);
        metadata.TryGetValue(BillingPeriodMetadataKey, out string? billingPeriod);
        metadata.TryGetValue(PlanCodeMetadataKey, out string? planCode);
        metadata.TryGetValue(RegistrationTokenMetadataKey, out string? registrationTokenText);
        metadata.TryGetValue(
            RegistrationCompletedMetadataKey,
            out string? registrationCompletedText);

        Guid.TryParse(registrationTokenText, out Guid registrationToken);
        bool.TryParse(registrationCompletedText, out bool registrationCompleted);

        return new SubscriptionPaymentResult
        {
            AccountType = accountType ?? string.Empty,
            AmountInMinorUnits = paymentIntent.Amount,
            BillingPeriod = billingPeriod ?? string.Empty,
            ClientSecret = paymentIntent.ClientSecret,
            Currency = paymentIntent.Currency,
            FailureMessage = paymentIntent.LastPaymentError?.Message,
            PaymentIntentId = paymentIntent.Id,
            PlanCode = planCode ?? string.Empty,
            RegistrationCompleted = registrationCompleted,
            RegistrationToken = registrationToken,
            Status = paymentIntent.Status switch
            {
                "succeeded" => SubscriptionPaymentStatus.Succeeded,
                "requires_action" => SubscriptionPaymentStatus.RequiresAction,
                "processing" => SubscriptionPaymentStatus.Processing,
                _ => SubscriptionPaymentStatus.Failed
            }
        };
    }
}
