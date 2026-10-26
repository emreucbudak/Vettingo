using FlashMediator;
using Vettingo.EvaluationService.Application.Repository;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.HasCv;

public sealed class HasCvQueryHandler(ICvRepository cvRepository) : IRequestHandler<HasCvQueryRequest, bool>
{
    public Task<bool> Handle(HasCvQueryRequest request, CancellationToken cancellationToken)
    {
        return cvRepository.ExistsByUserIdAsync(request.UserId, cancellationToken);
    }
}
