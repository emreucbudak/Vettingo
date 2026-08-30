using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Repository;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetByType;

public sealed class GetPlansByTypeQueryHandler(
    IPlanRepository planRepository,
    ILogger<GetPlansByTypeQueryHandler> logger)
    : IRequestHandler<GetPlansByTypeQueryRequest, IReadOnlyList<GetPlansByTypeQueryResponse>>
{
    public async Task<IReadOnlyList<GetPlansByTypeQueryResponse>> Handle(
        GetPlansByTypeQueryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{HandlerName} isteği işleniyor. PlanType: {PlanType}",
            nameof(GetPlansByTypeQueryHandler),
            request.PlanType);

        IReadOnlyList<PlanEntity> plans = await planRepository.GetPlansByTypeAsync(
            request.PlanType,
            cancellationToken);

        return plans
            .Select(plan => new GetPlansByTypeQueryResponse
            {
                Id = plan.Id,
                PlanName = plan.PlanName,
                Price = plan.Price,
                PlanType = plan.PlanType,
                Properties = plan.PlanProperties
                    .Select(property => new GetPlansByTypePropertyResponse
                    {
                        Name = property.PropertiesName,
                        Count = property.Count
                    })
                    .ToList()
            })
            .ToList();
    }
}
