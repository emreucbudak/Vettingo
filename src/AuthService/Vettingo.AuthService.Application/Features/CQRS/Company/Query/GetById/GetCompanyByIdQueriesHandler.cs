using FlashMediator;
using Vettingo.AuthService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById
{
    public class GetCompanyByIdQueriesHandler(ICompanyRepository companyRepository, ILogger<GetCompanyByIdQueriesHandler> logger) : IRequestHandler<GetCompanyByIdQueriesRequest, GetCompanyByIdQueriesResponse>
    {
        public async Task<GetCompanyByIdQueriesResponse> Handle(GetCompanyByIdQueriesRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetCompanyByIdQueriesHandler));
            var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new NotFoundException("Şirket bulunamadı");
            }

            return new GetCompanyByIdQueriesResponse
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyDescription = company.CompanyDescription,
                CompanyPhone = company.CompanyPhone,
                CompanyEmail = company.CompanyEmail,
                CompanyAddress = company.CompanyAddress
            };
        }
    }
}



