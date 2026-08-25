using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Application.Repository
{
    public interface IPlanRepository
    {
        Task AddPlanAsync(Plan plan);
        void UpdatePlan(Plan plan);
        void DeletePlan(Plan plan);
        Task<Plan?> GetPlanByIdAsync(int planId);
        Task<IEnumerable<Plan>> GetAllPlansAsync();
        Task<int> SaveChangesAsync();
    }
}
