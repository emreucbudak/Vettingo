namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById
{
    public class GetCompanyByIdQueriesResponse
    {
        public Guid Id { get; init; }
        public string CompanyName { get; init; } = string.Empty;
        public string CompanyDescription { get; init; } = string.Empty;
        public string CompanySector { get; init; } = string.Empty;
        public string CompanyWebsite { get; init; } = string.Empty;
        public string CompanySize { get; init; } = string.Empty;
        public string CompanyAddress { get; init; } = string.Empty;
    }
}
