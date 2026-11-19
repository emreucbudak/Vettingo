using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Persistence.DbContext
{
    public class IdentityDbContext : IdentityDbContext<User, Role, Guid>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {
        }

        protected IdentityDbContext()
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Company> Companys { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        }
    }
}
