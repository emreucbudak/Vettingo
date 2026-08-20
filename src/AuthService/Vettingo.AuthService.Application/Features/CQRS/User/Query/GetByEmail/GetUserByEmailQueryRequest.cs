using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Users.Query.GetByEmail
{
    public class GetUserByEmailQueryRequest : IRequest<GetUserByEmailQueryResponse>
    {
        public string Email { get; init; } = string.Empty;
    }
}
