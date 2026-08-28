using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.Persistence.Repository
{
    public class PlanRepository(SubscriptionDbContext context) : IPlanRepository
    {
        private DbSet<Plan> PlanSet => context.Set<Plan>();

        public async Task AddPlanAsync(Plan plan, CancellationToken cancellationToken = default)
        {
            await PlanSet.AddAsync(plan, cancellationToken);
        }

        public void DeletePlan(Plan plan)
        {
            PlanSet.Remove(plan);
        }

        public async Task<IReadOnlyList<Plan>> GetAllPlansAsync(CancellationToken cancellationToken = default)
        {
            return await PlanSet
                .Include(plan => plan.PlanProperties)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Plan?> GetPlanByIdAsync(int planId, CancellationToken cancellationToken = default)
        {
            return await PlanSet
                .Include(plan => plan.PlanProperties)
                .FirstOrDefaultAsync(plan => plan.Id == planId, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }

        public void UpdatePlan(Plan plan)
        {
            PlanSet.Update(plan);
        }
    }
}
