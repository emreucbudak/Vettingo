namespace Vettingo.AuthService.Application.Features.CQRS.Auth.RefreshToken
{
    public record RefreshTokenCommandResponse
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
