using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.UpdateCompanyJobPostingCount;

public sealed record UpdateCompanyJobPostingCountCommandRequest : IRequest
{
    public Guid CompanyId { get; init; }
}
