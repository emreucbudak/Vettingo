namespace Vettingo.JobService.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
}