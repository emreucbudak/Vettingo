using FlashMediator;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany
{
    public class DeleteCompanyCommandHandler(ICompanyRepository company) : IRequestHandler<DeleteCompanyCommandRequest>
    {
        public async Task Handle(DeleteCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var selectedCompany = await company.GetCompanyByIdAsync(request.CompanyId);
             company.DeleteCompany(selectedCompany);
            await company.SaveChangesAsync();
        }
    }
}
