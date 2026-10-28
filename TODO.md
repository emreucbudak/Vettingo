# Vettingo yapılacaklar

Kullanıcı "ne eksiğimiz kaldı?" diye sorduğunda bu listedeki açık maddeleri de belirt.

- [ ] Testleri birlikte gözden geçir.

- [ ] Mobil uygulamadaki state yönetimini düzelt.

- [ ] Mobilde gerçek API kayıt akışlarına geç.

- [ ] Asenkron mesajlarda idempotency kaydı ve kontrolünü ayarla; aynı mesaj tekrar işlendiğinde aynı işlemin yeniden uygulanmasını önle.

- [ ] Eski bildirimlerin günlük olarak veritabanından silinmesini değerlendir ve planla.
  - Durum: Kullanıcının isteğiyle ertelendi; son tarih ve zamanlanmış hatırlatma yok.
  - Amaç: Bildirimlerin süresiz birikmesini önlemek; temizliği `CreatedAt` üzerinden yapmak.
  - Saklama süresi henüz kararlaştırılmadı; 30 gün yalnızca konuşulan örnekti.
  - `pg_cron` ile .NET `BackgroundService` seçeneklerini çoklu uygulama instance'larını dikkate alarak değerlendir. PostgreSQL ortamı destekliyorsa `pg_cron` öncelikli seçenek.
  - `pg_cron` seçilirse Docker imajı, başlangıç ayarları, ilk kurulum SQL'i ve bildirim tablosunun oluşturulma sırası birlikte ele alınmalı.
  - Bu maddeyi kaydetmek, kurulumu başlatma onayı değildir; kullanıcı yeniden istediğinde ele alınacak.

- [x] Kişiselleştirilmiş ilan kartlarına şirket adının nasıl getirileceğini yeniden değerlendir.
  - Seçilen çözüm: AuthService, Company ve HR tokenlarına `companyName` claim'i ekler. JobService ilan oluştururken şirket kimliğini ve adını token'dan alıp `JobPosting` üzerinde saklar; GetAll response'ları ve mobil kart bu adı kullanır.
  - Kullanıcıya göre şirket adı sonradan değişmeyecek; ayrıca şirket adı mesajlaşması gerekmiyor.
  - Veritabanında eski ilan kaydı bulunmadığından şirket adı doldurma işlemi gerekmiyor; yeni ilanlar şirket adıyla kaydedilir.
