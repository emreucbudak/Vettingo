using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.Persistence.Repository;

public class CandidateSubscriptionRepository(SubscriptionDbContext context)
    : ICandidateSubscriptionRepository
{
    private DbSet<CandidateSubscription> SubscriptionSet =>
        context.Set<CandidateSubscription>();

    public async Task AddCandidateSubscriptionAsync(
        CandidateSubscription subscription,
        CancellationToken cancellationToken = default)
    {
        await SubscriptionSet.AddAsync(subscription, cancellationToken);
    }

    public void DeleteCandidateSubscription(CandidateSubscription subscription)
    {
        SubscriptionSet.Remove(subscription);
    }

    public async Task<IReadOnlyList<CandidateSubscription>> GetAllCandidateSubscriptionsAsync(
        CancellationToken cancellationToken = default)
    {
        return await SubscriptionSet
            .Include(subscription => subscription.Plan)
            .ThenInclude(plan => plan!.PlanProperties)
            .OrderByDescending(subscription => subscription.StartDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CandidateSubscription>> GetCandidateSubscriptionsByCandidateIdAsync(
        Guid candidateId,
        CancellationToken cancellationToken = default)
    {
        return await SubscriptionSet
            .Include(subscription => subscription.Plan)
            .ThenInclude(plan => plan!.PlanProperties)
            .Where(subscription => subscription.CandidateId == candidateId)
            .OrderByDescending(subscription => subscription.StartDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<CandidateSubscription?> GetCandidateSubscriptionByIdAsync(
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

    public void UpdateCandidateSubscription(CandidateSubscription subscription)
    {
        SubscriptionSet.Update(subscription);
    }
}
