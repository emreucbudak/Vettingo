using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Infrastructure.Messaging;

public sealed class CompanySubscriptionRequestedConsumer(
    IPlanRepository planRepository,
    IMediator mediator,
    ILogger<CompanySubscriptionRequestedConsumer> logger)
    : ICapSubscribe
{
    [CapSubscribe(
        CompanySubscriptionRequestedMessage.TopicName,
        Group = "vettingo.subscription-service")]
    public async Task HandleAsync(
        CompanySubscriptionRequestedMessage message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Şirket abonelik eventi işleniyor. CompanyId: {CompanyId}, PlanCode: {PlanCode}",
            message.CompanyId,
            message.PlanCode);

        string normalizedPlanCode = NormalizePlanCode(message.PlanCode);
        IReadOnlyList<Plan> employerPlans = await planRepository.GetPlansByTypeAsync(
            PlanType.Employer,
            cancellationToken);
        Plan plan = employerPlans.FirstOrDefault(
            candidate => NormalizePlanCode(candidate.PlanName) == normalizedPlanCode)
            ?? throw new NotFoundException(
                $"'{message.PlanCode}' kodlu işveren planı bulunamadı.");

        await mediator.Send(
            new CreateCompanySubscriptionCommandRequest
            {
                CompanyId = message.CompanyId,
                PlanId = plan.Id,
                StartDate = message.StartDateUtc,
                EndDate = message.EndDateUtc
            },
            cancellationToken);
    }

    private static string NormalizePlanCode(string value)
    {
        return new string(
            value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
    }
}
