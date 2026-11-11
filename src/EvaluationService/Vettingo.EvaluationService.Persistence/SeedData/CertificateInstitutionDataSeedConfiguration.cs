using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Persistence.SeedData;

public sealed class CertificateInstitutionDataSeedConfiguration : IEntityTypeConfiguration<CertificateInstitution>
{
    public void Configure(EntityTypeBuilder<CertificateInstitution> builder)
    {
        builder.HasData(
            new CertificateInstitution { Id = 1, InstitutionName = "Udemy" },
            new CertificateInstitution { Id = 2, InstitutionName = "BTK Akademi" },
            new CertificateInstitution { Id = 3, InstitutionName = "Turkcell Akademi (Geleceği Yazanlar)" },
            new CertificateInstitution { Id = 4, InstitutionName = "ODTÜ Bilgeİş" },
            new CertificateInstitution { Id = 5, InstitutionName = "Enstitü İstanbul İSMEK" },
            new CertificateInstitution { Id = 6, InstitutionName = "Coursera" },
            new CertificateInstitution { Id = 7, InstitutionName = "edX" },
            new CertificateInstitution { Id = 8, InstitutionName = "Microsoft" },
            new CertificateInstitution { Id = 9, InstitutionName = "Google" },
            new CertificateInstitution { Id = 10, InstitutionName = "Cisco Networking Academy" },
            new CertificateInstitution { Id = 11, InstitutionName = "Diğer" });
    }
}
