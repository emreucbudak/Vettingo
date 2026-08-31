namespace Vettingo.AuthService.Application.Payment;

public interface ISubscriptionPaymentGateway
{
    Task<SubscriptionPaymentResult> ConfirmAsync(
        SubscriptionPaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<SubscriptionPaymentResult> GetAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);

    Task MarkRegistrationCompletedAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);
}

public sealed record SubscriptionPaymentRequest
{
    public required string ConfirmationTokenId { get; init; }
    public required string AccountType { get; init; }
    public required string PlanCode { get; init; }
    public required string BillingPeriod { get; init; }
    public required Guid RegistrationToken { get; init; }
    public required long AmountInMinorUnits { get; init; }
    public required string Currency { get; init; }
}

public sealed record SubscriptionPaymentResult
{
    public required string PaymentIntentId { get; init; }
    public required SubscriptionPaymentStatus Status { get; init; }
    public string? ClientSecret { get; init; }
    public string? FailureMessage { get; init; }
    public required string AccountType { get; init; }
    public required string PlanCode { get; init; }
    public required string BillingPeriod { get; init; }
    public required Guid RegistrationToken { get; init; }
    public required long AmountInMinorUnits { get; init; }
    public required string Currency { get; init; }
    public bool RegistrationCompleted { get; init; }
}

public enum SubscriptionPaymentStatus
{
    RequiresAction,
    Processing,
    Succeeded,
    Failed
}
