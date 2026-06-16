using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany
{
    public class UpdateCompanyCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = string.Empty;
        public string CompanyDescription { get; init; } = string.Empty;
        public string CompanyPhone { get; init; } = string.Empty;
        public string CompanyEmail { get; init; } = string.Empty;
        public string CompanyAddress { get; init; } = string.Empty;
    }
}
