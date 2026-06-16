using FlashMediator;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany
{
    public class DeleteCompanyCommandHandler(ICompanyRepository companyRepository) : IRequestHandler<DeleteCompanyCommandRequest>
    {
        public async Task Handle(DeleteCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new Exception("Company not found");
            }

            companyRepository.DeleteCompany(company);
            await companyRepository.SaveChangesAsync();
        }
    }
}
