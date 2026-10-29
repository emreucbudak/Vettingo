using FlashMediator;
using Vettingo.AuthService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.UnitOfWork;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany
{
    public class UpdateCompanyCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCompanyCommandHandler> logger) : IRequestHandler<UpdateCompanyCommandRequest>
    {
        public async Task Handle(UpdateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateCompanyCommandHandler));
            var company = await unitOfWork.CompanyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new NotFoundException("Şirket bulunamadı");
            }

            company.setCompanyName(request.CompanyName);
            company.setCompanyDescription(request.CompanyDescription);
            company.setCompanySector(request.CompanySector);
            company.setCompanyWebsite(request.CompanyWebsite);
            company.setCompanySize(request.CompanySize);
            company.setCompanyAddress(request.CompanyAddress);

            unitOfWork.CompanyRepository.UpdateCompany(company);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}



