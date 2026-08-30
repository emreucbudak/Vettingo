namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerTempRegister
{
    public sealed record EmployerTempRegistrationData
    {
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public string CompanyName { get; init; } = string.Empty;
        public string CompanyDescription { get; init; } = string.Empty;
        public string CompanyPhone { get; init; } = string.Empty;
        public string CompanyEmail { get; init; } = string.Empty;
        public string CompanyAddress { get; init; } = string.Empty;
    }
}
