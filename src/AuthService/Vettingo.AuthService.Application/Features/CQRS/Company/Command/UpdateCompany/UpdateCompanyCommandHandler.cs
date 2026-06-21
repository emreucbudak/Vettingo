using FlashMediator;
using Vettingo.AuthService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany
{
    public class UpdateCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<UpdateCompanyCommandHandler> logger) : IRequestHandler<UpdateCompanyCommandRequest>
    {
        public async Task Handle(UpdateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateCompanyCommandHandler));
            var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new NotFoundException("Şirket bulunamadı");
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



