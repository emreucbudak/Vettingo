namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById
{
    public sealed record GetCompanyByIdQueriesResponse
    {
        public Guid Id { get; init; }
        public string CompanyName { get; init; } = string.Empty;
        public string? CompanyDescription { get; init; }
        public string? CompanySector { get; init; }
        public string? CompanyWebsite { get; init; }
        public string? CompanySize { get; init; }
        public string? CompanyAddress { get; init; }
    }
}
