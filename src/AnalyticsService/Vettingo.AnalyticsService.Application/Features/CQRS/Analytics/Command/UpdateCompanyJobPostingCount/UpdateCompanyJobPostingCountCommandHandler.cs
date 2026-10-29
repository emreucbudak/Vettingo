using FlashMediator;
using Vettingo.AnalyticsService.Application.Exceptions;
using Vettingo.AnalyticsService.Application.UnitOfWork;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.UpdateCompanyJobPostingCount;

public sealed class UpdateCompanyJobPostingCountCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCompanyJobPostingCountCommandRequest>
{
    public async Task Handle(
        UpdateCompanyJobPostingCountCommandRequest request,
        CancellationToken cancellationToken)
    {
        if (request.EventId == Guid.Empty)
            throw new BadRequestException("EventId boş olamaz.");
        if (request.CompanyId == Guid.Empty)
            throw new BadRequestException("CompanyId boş olamaz.");

        if (await unitOfWork.ProcessedEventRepository.ExistsAsync(request.EventId, cancellationToken))
            return;

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await unitOfWork.ProcessedEventRepository.AddAsync(new ProcessedEvent(request.EventId), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            int affectedRows = await unitOfWork.AnalyticsRepository
                .IncrementCompanyJobPostingCountAsync(request.CompanyId, cancellationToken);
            if (affectedRows == 0)
                throw new NotFoundException("Şirket istatistiği bulunamadı.");

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
