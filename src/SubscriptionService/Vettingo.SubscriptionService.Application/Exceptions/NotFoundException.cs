using Vettingo.SubscriptionService.Application.Bases;

namespace Vettingo.SubscriptionService.Application.Exceptions;

public class NotFoundException(string message) : BaseException(message);
