using FlashMediator;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Command.CreateCompanyStatics;

public sealed class CreateCompanyStaticsCommandHandler(IAnalyticsRepository analyticsRepository)
    : IRequestHandler<CreateCompanyStaticsCommandRequest>
{
    public async Task Handle(
        CreateCompanyStaticsCommandRequest request,
        CancellationToken cancellationToken)
    {
        var statistic = new CompanyStatistic(request.CompanyId);

        await analyticsRepository.AddCompanyStatisticAsync(statistic, cancellationToken);
        await analyticsRepository.SaveChangesAsync();
    }
}
