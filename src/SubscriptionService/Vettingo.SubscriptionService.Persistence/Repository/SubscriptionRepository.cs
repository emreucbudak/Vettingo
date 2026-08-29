using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.Persistence.Repository
{
    public class SubscriptionRepository(SubscriptionDbContext context) : ISubscriptionRepository
    {
        private DbSet<Subscription> SubscriptionSet => context.Set<Subscription>();

        public async Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
        {
            await SubscriptionSet.AddAsync(subscription, cancellationToken);
        }

        public void DeleteSubscription(Subscription subscription)
        {
            SubscriptionSet.Remove(subscription);
        }

        public async Task<IReadOnlyList<Subscription>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .OrderByDescending(subscription => subscription.StartDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Subscription>> GetSubscriptionsByCompanyIdAsync(
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

        public async Task<Subscription?> GetSubscriptionByIdAsync(
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

        public void UpdateSubscription(Subscription subscription)
        {
            SubscriptionSet.Update(subscription);
        }
    }
}
