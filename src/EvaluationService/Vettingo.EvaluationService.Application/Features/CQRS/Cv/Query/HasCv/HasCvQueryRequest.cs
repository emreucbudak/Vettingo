using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.HasCv;

public sealed record HasCvQueryRequest(Guid UserId) : IRequest<bool>;
