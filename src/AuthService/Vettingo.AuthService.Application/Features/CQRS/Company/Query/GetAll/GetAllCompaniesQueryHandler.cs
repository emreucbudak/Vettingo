using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetAll
{
    public class GetAllCompaniesQueryHandler(ICompanyRepository companyRepository, ILogger<GetAllCompaniesQueryHandler> logger) : IRequestHandler<GetAllCompaniesQueryRequest, IEnumerable<GetAllCompaniesQueryResponse>>
    {
        public async Task<IEnumerable<GetAllCompaniesQueryResponse>> Handle(GetAllCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllCompaniesQueryHandler));
            var companies = await companyRepository.GetAllCompaniesAsync();

            return companies.Select(company => new GetAllCompaniesQueryResponse
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyDescription = company.CompanyDescription,
                CompanyPhone = company.CompanyPhone,
                CompanyEmail = company.CompanyEmail,
                CompanyAddress = company.CompanyAddress
            });
        }
    }
}


