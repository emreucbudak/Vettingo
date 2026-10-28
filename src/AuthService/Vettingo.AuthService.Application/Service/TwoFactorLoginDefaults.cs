namespace Vettingo.AuthService.Application.Service;

public static class TwoFactorLoginDefaults
{
    public static string CacheKey(int code) => $"two-factor-login:{code}";
}
