using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        ConfigureDictionary(builder.Property(cv => cv.Skills));
        ConfigureDictionary(builder.Property(cv => cv.Languages));

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

    private static void ConfigureDictionary<TValue>(PropertyBuilder<Dictionary<string, TValue>> property)
    {
        property.HasColumnType("jsonb")
            .HasConversion(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<Dictionary<string, TValue>>(json, (JsonSerializerOptions?)null)!)
            .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, TValue>>(
                (left, right) => left == right ||
                    (left != null && right != null && left.Count == right.Count &&
                     left.All(item => right.ContainsKey(item.Key) &&
                         EqualityComparer<TValue>.Default.Equals(right[item.Key], item.Value))),
                value => value.OrderBy(item => item.Key, StringComparer.Ordinal)
                    .Aggregate(0, (hash, item) => HashCode.Combine(hash, item.Key, item.Value)),
                value => new Dictionary<string, TValue>(value)));
    }
}
