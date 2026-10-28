namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

public class TwoFactorLoginCommandResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
