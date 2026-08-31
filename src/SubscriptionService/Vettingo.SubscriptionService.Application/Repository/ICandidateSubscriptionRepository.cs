using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Application.Repository;

public interface ICandidateSubscriptionRepository
{
    Task AddCandidateSubscriptionAsync(
        CandidateSubscription subscription,
        CancellationToken cancellationToken = default);

    void UpdateCandidateSubscription(CandidateSubscription subscription);
    void DeleteCandidateSubscription(CandidateSubscription subscription);

    Task<CandidateSubscription?> GetCandidateSubscriptionByIdAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CandidateSubscription>> GetCandidateSubscriptionsByCandidateIdAsync(
        Guid candidateId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CandidateSubscription>> GetAllCandidateSubscriptionsAsync(
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
