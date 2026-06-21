using FlashMediator;
using Vettingo.AuthService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany
{
    public class DeleteCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<DeleteCompanyCommandHandler> logger) : IRequestHandler<DeleteCompanyCommandRequest>
    {
        public async Task Handle(DeleteCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteCompanyCommandHandler));
            var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new NotFoundException("Şirket bulunamadı");
            }

            companyRepository.DeleteCompany(company);
            await companyRepository.SaveChangesAsync();
        }
    }
}



