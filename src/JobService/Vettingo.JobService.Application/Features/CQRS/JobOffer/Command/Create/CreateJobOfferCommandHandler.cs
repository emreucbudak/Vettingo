using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Command.Create;

public sealed class CreateJobOfferCommandHandler : IRequestHandler<CreateJobOfferCommandRequest>
{
    public Task Handle(CreateJobOfferCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
