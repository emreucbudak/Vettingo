namespace Vettingo.AuthService.Application.Service;

public static class ResetPasswordDefaults
{
    public static string CacheKey(int code) => $"reset-password:{code}";
}
