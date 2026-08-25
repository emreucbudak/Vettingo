using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Application.Repository
{
    public interface ISubscriptionRepository
    {
        Task AddSubscriptionAsync(Subscription subscription);
        void UpdateSubscription(Subscription subscription);
        void DeleteSubscription(Subscription subscription);
        Task<Subscription?> GetSubscriptionByIdAsync(Guid subscriptionId);
        Task<IEnumerable<Subscription>> GetSubscriptionsByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<int> SaveChangesAsync();
    }
}
