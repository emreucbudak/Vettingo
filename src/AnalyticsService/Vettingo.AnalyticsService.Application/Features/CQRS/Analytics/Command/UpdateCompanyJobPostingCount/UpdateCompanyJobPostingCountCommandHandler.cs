using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.UpdateCompanyJobPostingCount;

public sealed class UpdateCompanyJobPostingCountCommandHandler
    : IRequestHandler<UpdateCompanyJobPostingCountCommandRequest>
{
    public Task Handle(
        UpdateCompanyJobPostingCountCommandRequest request,
        CancellationToken cancellationToken)
    {
        // İlan sayısını güncelleme işlemi daha sonra eklenecek.
        return Task.CompletedTask;
    }
}
