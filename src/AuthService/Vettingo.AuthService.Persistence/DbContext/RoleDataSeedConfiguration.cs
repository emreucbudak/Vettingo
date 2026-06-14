using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Persistence.DbContext
{
    public class RoleDataSeedConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                    Name = "Admin",
                    NormalizedName = "ADMIN" 
                },
                new Role
                {
                    Id = Guid.Parse("b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e"),
                    Name = "Worker",
                    NormalizedName = "WORKER"
                },
                new Role
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f"),
                    Name = "Company",
                    NormalizedName = "COMPANY"
                },
                new Role
                {
                    Id = Guid.Parse("d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a"),
                    Name = "Human Resources",
                    NormalizedName = "HUMAN RESOURCES"
                }
            );
        }
    }
}