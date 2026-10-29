using FlashMediator;
using Vettingo.AuthService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.UnitOfWork;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany
{
    public class DeleteCompanyCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCompanyCommandHandler> logger) : IRequestHandler<DeleteCompanyCommandRequest>
    {
        public async Task Handle(DeleteCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteCompanyCommandHandler));
            var company = await unitOfWork.CompanyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new NotFoundException("Şirket bulunamadı");
            }

            unitOfWork.CompanyRepository.DeleteCompany(company);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}



