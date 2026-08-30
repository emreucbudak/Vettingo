using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Application.Repository
{
    public interface IPlanRepository
    {
        Task AddPlanAsync(Plan plan, CancellationToken cancellationToken = default);
        void UpdatePlan(Plan plan);
        void DeletePlan(Plan plan);
        Task<Plan?> GetPlanByIdAsync(int planId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Plan>> GetPlansByTypeAsync(
            PlanType planType,
            CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
