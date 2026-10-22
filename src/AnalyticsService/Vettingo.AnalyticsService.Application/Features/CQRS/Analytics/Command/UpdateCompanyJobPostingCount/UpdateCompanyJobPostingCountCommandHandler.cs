using FlashMediator;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.UpdateCompanyJobPostingCount;

public sealed class UpdateCompanyJobPostingCountCommandHandler(IAnalyticsRepository analyticsRepository)
    : IRequestHandler<UpdateCompanyJobPostingCountCommandRequest>
{
    public Task Handle(
        UpdateCompanyJobPostingCountCommandRequest request,
        CancellationToken cancellationToken)
    {
        return analyticsRepository.IncrementCompanyJobPostingCountAsync(request.CompanyId, cancellationToken);
    }
}
