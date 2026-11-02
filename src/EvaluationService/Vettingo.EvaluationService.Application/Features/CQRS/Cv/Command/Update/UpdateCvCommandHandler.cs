using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;

public sealed class UpdateCvCommandHandler : IRequestHandler<UpdateCvCommandRequest>
{
    public Task Handle(UpdateCvCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
