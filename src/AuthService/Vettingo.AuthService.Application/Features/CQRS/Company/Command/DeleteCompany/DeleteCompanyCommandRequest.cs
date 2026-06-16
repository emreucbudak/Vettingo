using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany
{
    public class DeleteCompanyCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
    }
}
