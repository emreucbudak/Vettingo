using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.Persistence.Repository
{
    public class SubscriptionRepository(SubscriptionDbContext context) : ISubscriptionRepository
    {
        private DbSet<Subscription> SubscriptionSet => context.Set<Subscription>();

        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            await SubscriptionSet.AddAsync(subscription);
        }

        public void DeleteSubscription(Subscription subscription)
        {
            SubscriptionSet.Remove(subscription);
        }

        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .OrderByDescending(subscription => subscription.StartDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByCompanyIdAsync(Guid companyId)
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .Where(subscription => subscription.CompanyId == companyId)
                .OrderByDescending(subscription => subscription.StartDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(Guid subscriptionId)
        {
            return await SubscriptionSet
                .Include(subscription => subscription.Plan)
                .ThenInclude(plan => plan!.PlanProperties)
                .FirstOrDefaultAsync(subscription => subscription.Id == subscriptionId);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateSubscription(Subscription subscription)
        {
            SubscriptionSet.Update(subscription);
        }
    }
}
