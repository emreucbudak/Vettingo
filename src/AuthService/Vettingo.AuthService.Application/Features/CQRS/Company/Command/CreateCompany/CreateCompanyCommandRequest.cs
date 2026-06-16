using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany
{
    public record CreateCompanyCommandRequest : IRequest
    {
        public string CompanyName { get; init; }
        public string CompanyDescription { get; init; }
        public string CompanyPhone { get; init; }
        public string CompanyEmail { get; init; }
        public string CompanyAddress { get; init; }
    }
}
