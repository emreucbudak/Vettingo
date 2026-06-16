using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany
{
    public record DeleteCompanyCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }

        public DeleteCompanyCommandRequest(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
