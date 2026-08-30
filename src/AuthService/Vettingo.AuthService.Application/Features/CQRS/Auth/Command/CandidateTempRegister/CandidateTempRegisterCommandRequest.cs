using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateTempRegister
{
    public sealed record CandidateTempRegisterCommandRequest
        : IRequest<CandidateTempRegisterCommandResponse>
    {
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
