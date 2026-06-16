using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById
{
    public class GetCompanyByIdQueriesRequest : IRequest<GetCompanyByIdQueriesResponse>
    {
        public Guid CompanyId { get; init; }
    }
}
