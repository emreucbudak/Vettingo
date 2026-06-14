using Microsoft.AspNetCore.Identity;

namespace Vettingo.AuthService.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
