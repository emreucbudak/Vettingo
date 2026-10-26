# Kullanıcının CV varlığını sorgulama

`GET /api/evaluations/cv/user/{userId:guid}/exists` ham JSON boolean döndürür:
CV kaydı varsa `true`, yoksa `false`. Sorgu, değerlendirme puanlarını veya yerel
mobil profil durumunu değil, `Cvs.UserId` alanını kontrol eder.

Endpoint Candidate rolünde Bearer token gerektirir. Controller route'tan gelen
`userId` GUID'sini doğrudan `HasCvQueryRequest` ile mediator'a iletir. GUID
doğrulaması CQRS validator'ında yapılır. Mevcut servis hız sınırı bu endpoint'e
de uygulanır; 429 bir CV yokluğu sonucu değildir.

Controller, `HasCvQueryRequest` üzerinden FlashMediator handler'ına gider.
`ICvRepository` implementasyonu `AnyAsync` kullanır; kullanıcıya ait CV içeriği
veya koleksiyonları yüklenmez. Sonuç cache'lenmez.

## Veritabanı

CV modeli daha önce EvaluationDbContext'e dahil değildi. CV tabloları ve
indeksleri için EF Core model eşlemesi eklendi. Veritabanı şema güncellemesi
projenin mevcut migration süreciyle yönetilir; bu değişiklik veritabanında
otomatik şema işlemi çalıştırmaz.

Bu endpoint CV oluşturmaz. Mobildeki mevcut CV kaydetme akışı yereldir;
backend'e CV kaydı yazılması ayrıca entegre edilmelidir.
