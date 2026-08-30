namespace Vettingo.AuthService.Application.Messaging;

public interface ICompanySubscriptionPublisher
{
    Task PublishSubscriptionRequestedAsync(
        CompanySubscriptionRequestedEvent message,
        CancellationToken cancellationToken = default);
}
