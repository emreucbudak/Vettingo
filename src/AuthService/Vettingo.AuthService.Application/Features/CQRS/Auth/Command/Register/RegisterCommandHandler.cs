using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register
{
    public class RegisterCommandHandler(UserManager<User> userManager, AuthBusinessRules businessRules, ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommandRequest>
    {
        public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(RegisterCommandHandler));

            await businessRules.IsThere(request.Email);
            await businessRules.IsRoleThere(request.Role);

            User user = new()
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                UserName = request.Email
            };

            await userManager.CreateAsync(user, request.Password);
        }
    }
}

