using Vettingo.EvaluationService.Application.Bases;

namespace Vettingo.EvaluationService.Application.Exceptions;

public class BusinessException(string message) : BaseException(message);
