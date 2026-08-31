using Vettingo.SubscriptionService.Application.Bases;

namespace Vettingo.SubscriptionService.Application.Exceptions;

public sealed class BusinessException(string message) : BaseException(message);
