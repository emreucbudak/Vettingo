using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;

public sealed class GetCompanyHiringVolumeQueryHandler(
    IAnalyticsRepository analyticsRepository,
    ILogger<GetCompanyHiringVolumeQueryHandler> logger)
    : IRequestHandler<GetCompanyHiringVolumeQueryRequest, IEnumerable<GetCompanyHiringVolumeQueryResponse>>
{
    public async Task<IEnumerable<GetCompanyHiringVolumeQueryResponse>> Handle(
        GetCompanyHiringVolumeQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Şirket {CompanyId} için son altı işe alım hacmi kaydı getiriliyor", request.CompanyId);
        var statistic = await analyticsRepository.GetCompanyStatisticByCompanyIdAsync(request.CompanyId, cancellationToken);
        if (statistic is null)
            return [];

        return statistic.GetLatestHiringVolumes().Select(volume => new GetCompanyHiringVolumeQueryResponse
        {
            Year = volume.Year,
            Month = volume.Month,
            HireCount = volume.HireCount
        }).ToList();
    }
}
