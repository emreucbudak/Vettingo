using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.UnitOfWork;
using JobOfferEntity = Vettingo.JobService.Domain.Entities.JobOffer;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Command.Create;

public sealed class CreateJobOfferCommandHandler(IJobUnitOfWork unitOfWork) : IRequestHandler<CreateJobOfferCommandRequest>
{
    public async Task Handle(CreateJobOfferCommandRequest request, CancellationToken cancellationToken)
    {
        var offers = await unitOfWork.OffersRepository.GetByIdAsync(request.OffersId, cancellationToken)
            ?? throw new NotFoundException("Teklif aşaması bulunamadı.");

        var jobOffer = new JobOfferEntity();
        jobOffer.CreateJobOffer(request.UserId, request.OffersId, request.JobPostingId, request.Salary);
        offers.AddJobOffer(jobOffer);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
