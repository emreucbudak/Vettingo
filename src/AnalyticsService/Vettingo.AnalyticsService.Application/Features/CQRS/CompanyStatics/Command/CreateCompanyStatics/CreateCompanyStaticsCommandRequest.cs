using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Command.CreateCompanyStatics;

public sealed record CreateCompanyStaticsCommandRequest : IRequest
{
    public Guid CompanyId { get; init; }
}
