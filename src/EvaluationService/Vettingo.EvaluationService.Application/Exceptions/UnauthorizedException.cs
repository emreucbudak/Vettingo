using Vettingo.EvaluationService.Application.Bases;

namespace Vettingo.EvaluationService.Application.Exceptions;

public class UnauthorizedException(string message) : BaseException(message);
