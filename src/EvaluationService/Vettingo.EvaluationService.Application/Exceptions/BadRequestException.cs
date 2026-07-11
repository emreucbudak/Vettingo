using Vettingo.EvaluationService.Application.Bases;

namespace Vettingo.EvaluationService.Application.Exceptions;

public class BadRequestException(string message) : BaseException(message);
