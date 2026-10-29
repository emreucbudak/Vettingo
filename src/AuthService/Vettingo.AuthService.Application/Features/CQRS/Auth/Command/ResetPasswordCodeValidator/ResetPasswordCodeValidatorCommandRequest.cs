using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordCodeValidator;

public record ResetPasswordCodeValidatorCommandRequest : IRequest<ResetPasswordCodeValidatorCommandResponse>;
