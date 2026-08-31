using Vettingo.SubscriptionService.Application.Bases;

namespace Vettingo.SubscriptionService.Application.Exceptions;

public sealed class BadRequestException(string message) : BaseException(message);
