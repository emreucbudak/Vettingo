using FlashMediator;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetAll
{
    public class GetAllCompaniesQueryHandler(ICompanyRepository companyRepository) : IRequestHandler<GetAllCompaniesQueryRequest, IEnumerable<GetAllCompaniesQueryResponse>>
    {
        public async Task<IEnumerable<GetAllCompaniesQueryResponse>> Handle(GetAllCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
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
