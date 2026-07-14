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

        public async Task EnsureEmailIsAvailable(string email)
        {
            if (await users.FindByEmailAsync(email) is not null)
            {
                throw new BusinessException("Bu e-posta adresi zaten kay\u0131tl\u0131");
            }
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

