using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetAll
{
    public class GetAllCompaniesQueryRequest : IRequest<IEnumerable<GetAllCompaniesQueryResponse>>
    {
    }
}
