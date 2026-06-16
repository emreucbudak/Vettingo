using FlashMediator;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany
{
    public class CreateCompanyCommandHandler(ICompanyRepository companyRepository) : IRequestHandler<CreateCompanyCommandRequest>
    {
        public async Task Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            AuthService.Domain.Entities.Company company = new AuthService.Domain.Entities.Company();
            company.UpdateCompany(request.CompanyName, request.CompanyDescription, request.CompanyPhone, request.CompanyEmail, request.CompanyAddress);
            await companyRepository.AddCompanyAsync(company);
            await companyRepository.SaveChangesAsync();





        }
    }
}
