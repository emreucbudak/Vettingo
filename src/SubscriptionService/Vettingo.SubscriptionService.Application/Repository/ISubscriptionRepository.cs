using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Application.Repository
{
    public interface ISubscriptionRepository
    {
        Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default);
        void UpdateSubscription(Subscription subscription);
        void DeleteSubscription(Subscription subscription);
        Task<Subscription?> GetSubscriptionByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Subscription>> GetSubscriptionsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Subscription>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
