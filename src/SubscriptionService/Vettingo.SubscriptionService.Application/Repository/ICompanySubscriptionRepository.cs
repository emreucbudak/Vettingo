using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Application.Repository
{
    public interface ICompanySubscriptionRepository
    {
        Task AddCompanySubscriptionAsync(CompanySubscription subscription, CancellationToken cancellationToken = default);
        void UpdateCompanySubscription(CompanySubscription subscription);
        void DeleteCompanySubscription(CompanySubscription subscription);
        Task<CompanySubscription?> GetCompanySubscriptionByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CompanySubscription>> GetCompanySubscriptionsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CompanySubscription>> GetAllCompanySubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
