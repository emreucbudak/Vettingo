using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyCandidateSource;

public sealed class GetCompanyCandidateSourceQueryHandler(
    IAnalyticsRepository analyticsRepository,
    ILogger<GetCompanyCandidateSourceQueryHandler> logger)
    : IRequestHandler<GetCompanyCandidateSourceQueryRequest, GetCompanyCandidateSourceQueryResponse>
{
    public async Task<GetCompanyCandidateSourceQueryResponse> Handle(
        GetCompanyCandidateSourceQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Şirket {CompanyId} için aday kaynakları getiriliyor", request.CompanyId);
        var source = await analyticsRepository.GetCandidateSourceByCompanyIdAsync(request.CompanyId, cancellationToken);

        return new GetCompanyCandidateSourceQueryResponse
        {
            Scout = source?.Scout ?? 0,
            Application = source?.Application ?? 0,
            CandidateCount = source?.CandidateCount ?? 0
        };
    }
}
