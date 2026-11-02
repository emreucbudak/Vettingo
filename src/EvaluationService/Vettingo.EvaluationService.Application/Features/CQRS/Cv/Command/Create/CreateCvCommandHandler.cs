using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Create;

public sealed class CreateCvCommandHandler : IRequestHandler<CreateCvCommandRequest, Guid>
{
    public Task<Guid> Handle(CreateCvCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
