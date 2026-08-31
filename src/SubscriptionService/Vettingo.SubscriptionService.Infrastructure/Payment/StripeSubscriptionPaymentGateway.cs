using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Payment;

namespace Vettingo.SubscriptionService.Infrastructure.Payment;

public sealed class StripeSubscriptionPaymentGateway(
    IOptions<StripeOptions> options,
    ILogger<StripeSubscriptionPaymentGateway> logger)
    : ISubscriptionPaymentGateway
{
    private const string AccountTypeMetadataKey = "vettingo_account_type";
    private const string BillingPeriodMetadataKey = "vettingo_billing_period";
    private const string PlanIdMetadataKey = "vettingo_plan_id";
    private const string AuthRegistrationRequestedMetadataKey =
        "vettingo_auth_registration_requested";
    private const string RegistrationTokenMetadataKey = "vettingo_registration_token";
    private const string SubscriberIdMetadataKey = "vettingo_subscriber_id";

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
                    Description = $"Vettingo {request.AccountType} {request.PlanName} {request.BillingPeriod} planı",
                    Metadata = new Dictionary<string, string>
                    {
                        [AccountTypeMetadataKey] = request.AccountType,
                        [BillingPeriodMetadataKey] = request.BillingPeriod,
                        [PlanIdMetadataKey] = request.PlanId.ToString(CultureInfo.InvariantCulture),
                        [AuthRegistrationRequestedMetadataKey] = bool.FalseString,
                        [RegistrationTokenMetadataKey] = request.RegistrationToken.ToString("D"),
                        [SubscriberIdMetadataKey] = request.SubscriberId.ToString("D")
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

    public async Task MarkAuthRegistrationRequestedAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        await CreateService().UpdateAsync(
            paymentIntentId,
            new PaymentIntentUpdateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [AuthRegistrationRequestedMetadataKey] = bool.TrueString
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
        metadata.TryGetValue(PlanIdMetadataKey, out string? planIdText);
        metadata.TryGetValue(RegistrationTokenMetadataKey, out string? registrationTokenText);
        metadata.TryGetValue(SubscriberIdMetadataKey, out string? subscriberIdText);
        metadata.TryGetValue(
            AuthRegistrationRequestedMetadataKey,
            out string? authRegistrationRequestedText);

        Guid.TryParse(registrationTokenText, out Guid registrationToken);
        Guid.TryParse(subscriberIdText, out Guid subscriberId);
        int.TryParse(
            planIdText,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int planId);
        bool.TryParse(
            authRegistrationRequestedText,
            out bool authRegistrationRequested);

        return new SubscriptionPaymentResult
        {
            AccountType = accountType ?? string.Empty,
            AmountInMinorUnits = paymentIntent.Amount,
            BillingPeriod = billingPeriod ?? string.Empty,
            ClientSecret = paymentIntent.ClientSecret,
            Currency = paymentIntent.Currency,
            FailureMessage = paymentIntent.LastPaymentError?.Message,
            PaymentIntentId = paymentIntent.Id,
            PlanId = planId,
            AuthRegistrationRequested = authRegistrationRequested,
            RegistrationToken = registrationToken,
            SubscriberId = subscriberId,
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
