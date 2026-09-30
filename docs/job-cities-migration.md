# JobService şehir tablosu

`JobPosting.Location` yerine zorunlu `int CityId` kullanılır. Oluşturma, güncelleme,
ilan yanıtları ve arama filtresi de `CityId` kullanır. Örnek: Ankara `6`, İstanbul `34`.
İstemcilerin `location` yerine `cityId` göndermesi gerekir.

`Cities` tablosunun alanları: `Id` (int), `CountryCode` (2 karakter), `CityName` (100 karakter).
Türkiye'nin 81 ili `CountryCode = TR` ile EF `HasData` üzerinden eklenir.
Seed kimlikleri 1–81 arasında sabittir ve plaka sırasındadır; global plaka kimliği değildir.
Yeni şehir kayıtlarının otomatik kimlikleri 82'den başlar, başka ülkeler eklenebilir.
Aynı ülkede aynı şehir adı tekrar edemez. İlanda kullanılan şehir silinemez.
İl listesi kaynağı: https://www.icisleri.gov.tr/valilikler

Uzaktan ilanlarda da şehir zorunludur. Örnek: İstanbul uzaktan ilanı için
`CityId = 34` ve `WorkingModel = Remote` kullanılır. Çalışma biçimi şehirden bağımsızdır.
Şehir CRUD uçları bu aşamada eklenmemiştir.

## Veritabanı geçişi

Şehir tablosu, seed verileri ve `JobPosting.CityId` ilişkisi EF Core migration ile
veritabanına uygulanacaktır. Ayrı SQL geçiş betiği kullanılmaz.

Migration henüz oluşturulmamıştır. JobService'in mevcut başlangıç akışı
`EnsureCreatedAsync` kullanır; migration geçişinde bu akış ayrıca düzenlenmelidir.