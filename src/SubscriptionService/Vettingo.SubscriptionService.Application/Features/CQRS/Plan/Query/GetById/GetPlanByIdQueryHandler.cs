using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetById;

public sealed class GetPlanByIdQueryHandler(
    IPlanRepository planRepository,
    ILogger<GetPlanByIdQueryHandler> logger)
    : IRequestHandler<GetPlanByIdQueryRequest, GetPlanByIdQueryResponse>
{
    public async Task<GetPlanByIdQueryResponse> Handle(
        GetPlanByIdQueryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{HandlerName} isteği işleniyor. PlanId: {PlanId}",
            nameof(GetPlanByIdQueryHandler),
            request.PlanId);

        PlanEntity plan = await planRepository.GetPlanByIdAsync(
            request.PlanId,
            cancellationToken)
            ?? throw new NotFoundException(
                $"{request.PlanId} kimlikli plan bulunamadı.");

        return new GetPlanByIdQueryResponse
        {
            Id = plan.Id,
            PlanName = plan.PlanName,
            Price = plan.Price,
            PlanType = plan.PlanType,
            Properties = plan.PlanProperties
                .Select(property => new GetPlanByIdPropertyResponse
                {
                    Name = property.PropertiesName,
                    Count = property.Count
                })
                .ToList()
        };
    }
}
