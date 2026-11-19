using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Persistence.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasIndex(x => x.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("EmailIndex");
    }
}
