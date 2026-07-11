using Vettingo.EvaluationService.Application.Bases;

namespace Vettingo.EvaluationService.Application.Exceptions;

public class NotFoundException(string message) : BaseException(message);
