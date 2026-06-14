using FlashMediator;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Register
{
    public class RegisterCommandHandler(UserManager<User> userManager,RoleManager<Role> roleManager,AuthBusinessRules businessRules) : IRequestHandler<RegisterCommandRequest>
    {
        public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            await businessRules.IsThere(request.Email);
            await businessRules.IsRoleThere(request.Role);
            User user = new User
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
