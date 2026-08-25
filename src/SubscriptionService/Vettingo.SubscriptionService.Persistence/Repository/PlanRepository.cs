using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.Persistence.Repository
{
    public class PlanRepository(SubscriptionDbContext context) : IPlanRepository
    {
        private DbSet<Plan> PlanSet => context.Set<Plan>();

        public async Task AddPlanAsync(Plan plan)
        {
            await PlanSet.AddAsync(plan);
        }

        public void DeletePlan(Plan plan)
        {
            PlanSet.Remove(plan);
        }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync()
        {
            return await PlanSet
                .Include(plan => plan.PlanProperties)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Plan?> GetPlanByIdAsync(int planId)
        {
            return await PlanSet
                .Include(plan => plan.PlanProperties)
                .FirstOrDefaultAsync(plan => plan.Id == planId);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdatePlan(Plan plan)
        {
            PlanSet.Update(plan);
        }
    }
}
