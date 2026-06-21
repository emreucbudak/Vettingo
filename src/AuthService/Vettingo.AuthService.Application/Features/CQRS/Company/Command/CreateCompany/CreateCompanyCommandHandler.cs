using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany
{
    public class CreateCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<CreateCompanyCommandHandler> logger) : IRequestHandler<CreateCompanyCommandRequest>
    {
        public async Task Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateCompanyCommandHandler));
            AuthService.Domain.Entities.Company company = new AuthService.Domain.Entities.Company();
            company.UpdateCompany(request.CompanyName, request.CompanyDescription, request.CompanyPhone, request.CompanyEmail, request.CompanyAddress);
            await companyRepository.AddCompanyAsync(company);
            await companyRepository.SaveChangesAsync();





        }
    }
}


