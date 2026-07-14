using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register
{
    public class RegisterCommandHandler(UserManager<User> userManager, AuthBusinessRules businessRules, ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommandRequest>
    {
        public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(RegisterCommandHandler));

            await businessRules.EnsureEmailIsAvailable(request.Email);
            bool roleExists = await businessRules.IsRoleThere(request.Role);
            if (!roleExists)
            {
                throw new BusinessException("Ge\u00e7ersiz hesap t\u00fcr\u00fc");
            }

            User user = new()
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                UserName = request.Email
            };

            IdentityResult createResult = await userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                throw new BusinessException(string.Join(" ", createResult.Errors.Select(error => error.Description)));
            }

            IdentityResult roleResult = await userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                throw new BusinessException(string.Join(" ", roleResult.Errors.Select(error => error.Description)));
            }
        }
    }
}

