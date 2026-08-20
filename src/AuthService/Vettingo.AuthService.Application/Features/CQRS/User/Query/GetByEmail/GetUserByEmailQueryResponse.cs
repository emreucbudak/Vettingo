namespace Vettingo.AuthService.Application.Features.CQRS.Users.Query.GetByEmail
{
    public class GetUserByEmailQueryResponse
    {
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public string? Biography { get; init; }
        public string? TargetRole { get; init; }
    }
}
