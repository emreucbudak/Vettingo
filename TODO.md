# Vettingo yapılacaklar

Kullanıcı "ne eksiğimiz kaldı?" diye sorduğunda bu listedeki açık maddeleri de belirt.

- [ ] Eski bildirimlerin günlük olarak veritabanından silinmesini değerlendir ve planla.
  - Durum: Kullanıcının isteğiyle ertelendi; son tarih ve zamanlanmış hatırlatma yok.
  - Amaç: Bildirimlerin süresiz birikmesini önlemek; temizliği `CreatedAt` üzerinden yapmak.
  - Saklama süresi henüz kararlaştırılmadı; 30 gün yalnızca konuşulan örnekti.
  - `pg_cron` ile .NET `BackgroundService` seçeneklerini çoklu uygulama instance'larını dikkate alarak değerlendir. PostgreSQL ortamı destekliyorsa `pg_cron` öncelikli seçenek.
  - `pg_cron` seçilirse Docker imajı, başlangıç ayarları, ilk kurulum SQL'i ve bildirim tablosunun oluşturulma sırası birlikte ele alınmalı.
  - Bu maddeyi kaydetmek, kurulumu başlatma onayı değildir; kullanıcı yeniden istediğinde ele alınacak.

- [ ] Kişiselleştirilmiş ilan kartlarına şirket adının nasıl getirileceğini yeniden değerlendir.
  - Durum: Kullanıcının isteğiyle ertelendi; kullanıcı tekrar haber verdiğinde ele alınacak. Henüz bir çözüm kararlaştırılmadı.
  - İhtiyaç: Mobil aday ekranındaki ilan kartında şirket adı da gösterilmeli; mevcut ilan response'unda şirket adı bulunmuyor.
  - Konuşulan seçenek: Kişiselleştirilmiş ilan oluşturulunca JobService, `CompanyId` ve `JobPostingId` ile CAP/RabbitMQ üzerinden şirket adı talebi yayınlar; AuthService şirket adını cevap mesajıyla gönderir; JobService adı ilgili `JobPosting` kaydına ekleyip ilan response'unda döner.
  - Kullanıcıya göre şirket adı sonradan değişmeyecek. Aynı ilan için tekrar isim talebi, mesaj gelmeden yapılan sorgular ve ismin hangi aşamada kaydedileceği yeniden değerlendirilmeli.
  - Bu not uygulama onayı değildir; şirket adı entegrasyonunu kullanıcı yeniden istediğinde ele al.
