using FlashMediator;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany
{
    public class UpdateCompanyCommandHandler(ICompanyRepository companyRepository) : IRequestHandler<UpdateCompanyCommandRequest>
    {
        public async Task Handle(UpdateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new Exception("Company not found");
            }

            company.setCompanyName(request.CompanyName);
            company.setCompanyDescription(request.CompanyDescription);
            company.setCompanyPhone(request.CompanyPhone);
            company.setCompanyEmail(request.CompanyEmail);
            company.setCompanyAddress(request.CompanyAddress);

            companyRepository.UpdateCompany(company);
            await companyRepository.SaveChangesAsync();
        }
    }
}
