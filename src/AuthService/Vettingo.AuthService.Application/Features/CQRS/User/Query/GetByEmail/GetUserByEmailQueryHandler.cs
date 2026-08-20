using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Rules;

namespace Vettingo.AuthService.Application.Features.CQRS.Users.Query.GetByEmail
{
    public class GetUserByEmailQueryHandler(
        AuthBusinessRules authBusinessRules,
        ILogger<GetUserByEmailQueryHandler> logger)
        : IRequestHandler<GetUserByEmailQueryRequest, GetUserByEmailQueryResponse>
    {
        public async Task<GetUserByEmailQueryResponse> Handle(
            GetUserByEmailQueryRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetUserByEmailQueryHandler));

            Domain.Entities.User user = await authBusinessRules.IsThere(request.Email);

            return new GetUserByEmailQueryResponse
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Biography = user.Biography,
                TargetRole = user.TargetRole
            };
        }
    }
}