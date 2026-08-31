using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.Persistence.Repository
{
    public class CompanySubscriptionRepository(SubscriptionDbContext context)
        : ICompanySubscriptionRepository
    {
        private DbSet<CompanySubscription> SubscriptionSet => context.Set<CompanySubscription>();

        public async Task AddCompanySubscriptionAsync(CompanySubscription subscription, CancellationToken cancellationToken = default)
        {
            await SubscriptionSet.AddAsync(subscription, cancellationToken);
        }

        public void DeleteCompanySubscription(CompanySubscription subscription)
        {
            SubscriptionSet.Remove(subscription);
        }

        public async Task<IReadOnlyList<CompanySubscription>> GetAllCompanySubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .OrderByDescending(subscription => subscription.StartDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CompanySubscription>> GetCompanySubscriptionsByCompanyIdAsync(
            Guid companyId,
            CancellationToken cancellationToken = default)
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .Where(subscription => subscription.CompanyId == companyId)
                .OrderByDescending(subscription => subscription.StartDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<CompanySubscription?> GetCompanySubscriptionByIdAsync(
            Guid subscriptionId,
            CancellationToken cancellationToken = default)
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .FirstOrDefaultAsync(
                    subscription => subscription.Id == subscriptionId,
                    cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }

        public void UpdateCompanySubscription(CompanySubscription subscription)
        {
            SubscriptionSet.Update(subscription);
        }
    }
}
