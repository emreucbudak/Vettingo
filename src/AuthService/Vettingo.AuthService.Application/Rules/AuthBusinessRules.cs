using Microsoft.AspNetCore.Identity;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Rules
{
    public class AuthBusinessRules(UserManager<User> user, RoleManager<Role> roleManager)
    {
        public async Task<bool> IsThere(string email)
        {
            User IsFind = await user.FindByEmailAsync(email);
            if (IsFind is null)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> IsRoleThere(string roleName)
        {
            Role role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> IsPasswordCorrect(User users, string password)
        {
            return await user.CheckPasswordAsync(users, password);

        }
    }
}
