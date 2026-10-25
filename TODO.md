# Vettingo yapılacaklar

Kullanıcı "ne eksiğimiz kaldı?" diye sorduğunda bu listedeki açık maddeleri de belirt.

- [ ] Eski bildirimlerin günlük olarak veritabanından silinmesini değerlendir ve planla.
  - Durum: Kullanıcının isteğiyle ertelendi; son tarih ve zamanlanmış hatırlatma yok.
  - Amaç: Bildirimlerin süresiz birikmesini önlemek; temizliği `CreatedAt` üzerinden yapmak.
  - Saklama süresi henüz kararlaştırılmadı; 30 gün yalnızca konuşulan örnekti.
  - `pg_cron` ile .NET `BackgroundService` seçeneklerini çoklu uygulama instance'larını dikkate alarak değerlendir. PostgreSQL ortamı destekliyorsa `pg_cron` öncelikli seçenek.
  - `pg_cron` seçilirse Docker imajı, başlangıç ayarları, ilk kurulum SQL'i ve bildirim tablosunun oluşturulma sırası birlikte ele alınmalı.
  - Bu maddeyi kaydetmek, kurulumu başlatma onayı değildir; kullanıcı yeniden istediğinde ele alınacak.
