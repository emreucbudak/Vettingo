namespace Vettingo.AuthService.Application.Service;

public static class TempRegisterDefaults
{
    public static string CacheKey(int code) => $"temp-register:{code}";
}
