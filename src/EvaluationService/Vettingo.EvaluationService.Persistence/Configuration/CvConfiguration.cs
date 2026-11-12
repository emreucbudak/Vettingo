using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Persistence.Configuration;

public sealed class CvConfiguration : IEntityTypeConfiguration<Cv>
{
    public void Configure(EntityTypeBuilder<Cv> builder)
    {
        builder.ToTable("Cvs");
        builder.HasKey(cv => cv.Id);
        builder.Property(cv => cv.TargetRole).IsRequired();
        builder.OwnsMany(cv => cv.Languages, language =>
        {
            language.ToTable("CvLanguages");
            language.WithOwner().HasForeignKey("CvId");
            language.HasKey(item => item.Id);
            language.Property(item => item.Id).ValueGeneratedNever();
            language.Property(item => item.LanguageLevel).HasConversion<string>();
        });

        builder.OwnsMany(cv => cv.Educations, education =>
        {
            education.ToTable("CvEducations");
            education.WithOwner().HasForeignKey("CvId");
            education.HasKey(item => item.Id);
        });
        builder.OwnsMany(cv => cv.Projects, project =>
        {
            project.ToTable("CvProjects");
            project.WithOwner().HasForeignKey("CvId");
            project.HasKey(item => item.Id);
        });
        builder.OwnsMany(cv => cv.Experiences, experience =>
        {
            experience.ToTable("CvExperiences");
            experience.WithOwner().HasForeignKey("CvId");
            experience.HasKey(item => item.Id);
        });
    }
}
