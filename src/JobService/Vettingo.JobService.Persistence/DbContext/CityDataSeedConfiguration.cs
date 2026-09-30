using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.DbContext;

public sealed class CityDataSeedConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(city => city.Id);
        builder.Property(city => city.Id).UseIdentityByDefaultColumn().HasIdentityOptions(startValue: 82);
        builder.Property(city => city.CountryCode).IsRequired().HasMaxLength(2);
        builder.Property(city => city.CityName).IsRequired().HasMaxLength(100);
        builder.HasIndex(city => new { city.CountryCode, city.CityName }).IsUnique();

        // Stable seed IDs; Turkey's provinces are ordered by license plate code.
        // Source: https://www.icisleri.gov.tr/valilikler
        builder.HasData(
            new City { Id = 1, CountryCode = "TR", CityName = "Adana" },
            new City { Id = 2, CountryCode = "TR", CityName = "Adıyaman" },
            new City { Id = 3, CountryCode = "TR", CityName = "Afyonkarahisar" },
            new City { Id = 4, CountryCode = "TR", CityName = "Ağrı" },
            new City { Id = 5, CountryCode = "TR", CityName = "Amasya" },
            new City { Id = 6, CountryCode = "TR", CityName = "Ankara" },
            new City { Id = 7, CountryCode = "TR", CityName = "Antalya" },
            new City { Id = 8, CountryCode = "TR", CityName = "Artvin" },
            new City { Id = 9, CountryCode = "TR", CityName = "Aydın" },
            new City { Id = 10, CountryCode = "TR", CityName = "Balıkesir" },
            new City { Id = 11, CountryCode = "TR", CityName = "Bilecik" },
            new City { Id = 12, CountryCode = "TR", CityName = "Bingöl" },
            new City { Id = 13, CountryCode = "TR", CityName = "Bitlis" },
            new City { Id = 14, CountryCode = "TR", CityName = "Bolu" },
            new City { Id = 15, CountryCode = "TR", CityName = "Burdur" },
            new City { Id = 16, CountryCode = "TR", CityName = "Bursa" },
            new City { Id = 17, CountryCode = "TR", CityName = "Çanakkale" },
            new City { Id = 18, CountryCode = "TR", CityName = "Çankırı" },
            new City { Id = 19, CountryCode = "TR", CityName = "Çorum" },
            new City { Id = 20, CountryCode = "TR", CityName = "Denizli" },
            new City { Id = 21, CountryCode = "TR", CityName = "Diyarbakır" },
            new City { Id = 22, CountryCode = "TR", CityName = "Edirne" },
            new City { Id = 23, CountryCode = "TR", CityName = "Elazığ" },
            new City { Id = 24, CountryCode = "TR", CityName = "Erzincan" },
            new City { Id = 25, CountryCode = "TR", CityName = "Erzurum" },
            new City { Id = 26, CountryCode = "TR", CityName = "Eskişehir" },
            new City { Id = 27, CountryCode = "TR", CityName = "Gaziantep" },
            new City { Id = 28, CountryCode = "TR", CityName = "Giresun" },
            new City { Id = 29, CountryCode = "TR", CityName = "Gümüşhane" },
            new City { Id = 30, CountryCode = "TR", CityName = "Hakkâri" },
            new City { Id = 31, CountryCode = "TR", CityName = "Hatay" },
            new City { Id = 32, CountryCode = "TR", CityName = "Isparta" },
            new City { Id = 33, CountryCode = "TR", CityName = "Mersin" },
            new City { Id = 34, CountryCode = "TR", CityName = "İstanbul" },
            new City { Id = 35, CountryCode = "TR", CityName = "İzmir" },
            new City { Id = 36, CountryCode = "TR", CityName = "Kars" },
            new City { Id = 37, CountryCode = "TR", CityName = "Kastamonu" },
            new City { Id = 38, CountryCode = "TR", CityName = "Kayseri" },
            new City { Id = 39, CountryCode = "TR", CityName = "Kırklareli" },
            new City { Id = 40, CountryCode = "TR", CityName = "Kırşehir" },
            new City { Id = 41, CountryCode = "TR", CityName = "Kocaeli" },
            new City { Id = 42, CountryCode = "TR", CityName = "Konya" },
            new City { Id = 43, CountryCode = "TR", CityName = "Kütahya" },
            new City { Id = 44, CountryCode = "TR", CityName = "Malatya" },
            new City { Id = 45, CountryCode = "TR", CityName = "Manisa" },
            new City { Id = 46, CountryCode = "TR", CityName = "Kahramanmaraş" },
            new City { Id = 47, CountryCode = "TR", CityName = "Mardin" },
            new City { Id = 48, CountryCode = "TR", CityName = "Muğla" },
            new City { Id = 49, CountryCode = "TR", CityName = "Muş" },
            new City { Id = 50, CountryCode = "TR", CityName = "Nevşehir" },
            new City { Id = 51, CountryCode = "TR", CityName = "Niğde" },
            new City { Id = 52, CountryCode = "TR", CityName = "Ordu" },
            new City { Id = 53, CountryCode = "TR", CityName = "Rize" },
            new City { Id = 54, CountryCode = "TR", CityName = "Sakarya" },
            new City { Id = 55, CountryCode = "TR", CityName = "Samsun" },
            new City { Id = 56, CountryCode = "TR", CityName = "Siirt" },
            new City { Id = 57, CountryCode = "TR", CityName = "Sinop" },
            new City { Id = 58, CountryCode = "TR", CityName = "Sivas" },
            new City { Id = 59, CountryCode = "TR", CityName = "Tekirdağ" },
            new City { Id = 60, CountryCode = "TR", CityName = "Tokat" },
            new City { Id = 61, CountryCode = "TR", CityName = "Trabzon" },
            new City { Id = 62, CountryCode = "TR", CityName = "Tunceli" },
            new City { Id = 63, CountryCode = "TR", CityName = "Şanlıurfa" },
            new City { Id = 64, CountryCode = "TR", CityName = "Uşak" },
            new City { Id = 65, CountryCode = "TR", CityName = "Van" },
            new City { Id = 66, CountryCode = "TR", CityName = "Yozgat" },
            new City { Id = 67, CountryCode = "TR", CityName = "Zonguldak" },
            new City { Id = 68, CountryCode = "TR", CityName = "Aksaray" },
            new City { Id = 69, CountryCode = "TR", CityName = "Bayburt" },
            new City { Id = 70, CountryCode = "TR", CityName = "Karaman" },
            new City { Id = 71, CountryCode = "TR", CityName = "Kırıkkale" },
            new City { Id = 72, CountryCode = "TR", CityName = "Batman" },
            new City { Id = 73, CountryCode = "TR", CityName = "Şırnak" },
            new City { Id = 74, CountryCode = "TR", CityName = "Bartın" },
            new City { Id = 75, CountryCode = "TR", CityName = "Ardahan" },
            new City { Id = 76, CountryCode = "TR", CityName = "Iğdır" },
            new City { Id = 77, CountryCode = "TR", CityName = "Yalova" },
            new City { Id = 78, CountryCode = "TR", CityName = "Karabük" },
            new City { Id = 79, CountryCode = "TR", CityName = "Kilis" },
            new City { Id = 80, CountryCode = "TR", CityName = "Osmaniye" },
            new City { Id = 81, CountryCode = "TR", CityName = "Düzce" });
    }
}