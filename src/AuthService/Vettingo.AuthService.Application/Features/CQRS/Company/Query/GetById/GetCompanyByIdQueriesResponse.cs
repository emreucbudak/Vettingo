namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById
{
    public class GetCompanyByIdQueriesResponse
    {
        public Guid Id { get; init; }
        public string CompanyName { get; init; } = string.Empty;
        public string CompanyDescription { get; init; } = string.Empty;
        public string CompanyPhone { get; init; } = string.Empty;
        public string CompanyEmail { get; init; } = string.Empty;
        public string CompanyAddress { get; init; } = string.Empty;
    }
}
