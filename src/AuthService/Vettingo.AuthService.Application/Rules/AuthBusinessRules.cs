using Microsoft.AspNetCore.Identity;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Rules
{
    public class AuthBusinessRules(UserManager<User> users, RoleManager<Role> roleManager)
    {
        public async Task<User> IsThere(string email)
        {
            User? user = await users.FindByEmailAsync(email);

            if (user is null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı");
            }

            return user;
        }

        public async Task<bool> IsRoleThere(string roleName)
        {
            Role? role = await roleManager.FindByNameAsync(roleName);
            return role is not null;
        }

        public async Task<bool> IsPasswordCorrect(User user, string password)
        {
            return await users.CheckPasswordAsync(user, password);
        }
    }
}

