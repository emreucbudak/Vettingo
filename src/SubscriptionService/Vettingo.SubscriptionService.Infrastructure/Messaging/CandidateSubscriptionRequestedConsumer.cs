using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Infrastructure.Messaging;

public sealed class CandidateSubscriptionRequestedConsumer(
    IPlanRepository planRepository,
    IMediator mediator,
    ILogger<CandidateSubscriptionRequestedConsumer> logger)
    : ICapSubscribe
{
    [CapSubscribe(
        CandidateSubscriptionRequestedMessage.TopicName,
        Group = "vettingo.subscription-service")]
    public async Task HandleAsync(
        CandidateSubscriptionRequestedMessage message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Aday abonelik eventi işleniyor. CandidateId: {CandidateId}, PlanCode: {PlanCode}",
            message.CandidateId,
            message.PlanCode);

        string normalizedPlanCode = NormalizePlanCode(message.PlanCode);
        IReadOnlyList<Plan> candidatePlans = await planRepository.GetPlansByTypeAsync(
            PlanType.Candidate,
            cancellationToken);
        Plan plan = candidatePlans.FirstOrDefault(
            candidate => NormalizePlanCode(candidate.PlanName) == normalizedPlanCode)
            ?? throw new NotFoundException(
                $"'{message.PlanCode}' kodlu aday planı bulunamadı.");

        await mediator.Send(
            new CreateCandidateSubscriptionCommandRequest
            {
                CandidateId = message.CandidateId,
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
