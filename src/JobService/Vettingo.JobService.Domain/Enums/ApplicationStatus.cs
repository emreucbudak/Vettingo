using System.Text.Json.Serialization;

namespace Vettingo.JobService.Domain.Enums
{
    public enum ApplicationStatus
    {
        [JsonStringEnumMemberName("Submitted")]
        Başvuruldu = 1,
        [JsonStringEnumMemberName("UnderReview")]
        Sınav = 2,
        [JsonStringEnumMemberName("Interview")]
        Mülakat = 3,
        [JsonStringEnumMemberName("Offer")]
        Teklif = 4,
        [JsonStringEnumMemberName("Rejected")]
        Elendi = 5,
        [JsonStringEnumMemberName("Hired")]
        İşeAlındı = 6
    }
}
