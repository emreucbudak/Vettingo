using FlashMediator;
using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById
{
    public class GetCompanyByIdQueriesHandler(ICompanyRepository companyRepository) : IRequestHandler<GetCompanyByIdQueriesRequest, GetCompanyByIdQueriesResponse>
    {
        public async Task<GetCompanyByIdQueriesResponse> Handle(GetCompanyByIdQueriesRequest request, CancellationToken cancellationToken)
        {
            var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId);

            if (company is null)
            {
                throw new Exception("Company not found");
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
