using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Persistence.SeedData
{
    public class CompanyDataSeedConfiguration : IEntityTypeConfiguration<Domain.Entities.Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(c => c.Id).ValueGeneratedNever();
        }
    }
}
