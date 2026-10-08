using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

public class PersonalizedJobPostings : BaseEntity
{
    public PersonalizedJobPostings()
    {
        SetId();
        SetCreatedAt();
    }

    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly PublishedDate { get; set; }
    public int CityId { get; set; }
    public City City { get; set; } = null!;
}
