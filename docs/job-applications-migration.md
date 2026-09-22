# Başvuruların JobService'e taşınması

JobPosting ve JobApplication ayrı aggregate root olarak JobDbContext veritabanındadır.
Başvuru oluşturma, listeleme ve durum güncelleme endpointleri /api/job-applications adresinde korunmuştur.
Gateway bu adresi JobService'e yönlendirir. ApplicationService artık solution, Docker Compose veya CI içinde bulunmaz.
Başvuru istatistikleri şirketin ilanlarına ait gerçek başvuru kayıtlarından hesaplanır; Rejected aktif sayıya dahil değildir.
CAP mesajı, ApplicationCount property’si ve mine/ids HTTP bağımlılığı kaldırılmıştır.
Eski JobPostings.ApplicationCount kolonu mevcut veritabanında kalabilir; yeni kod bu kolonu kullanmaz.
Başvurusu bulunan ilanların fiziksel silinmesi foreign key ile engellenir; ilan kapatılabilir.

## Mevcut verileri koruyarak geçiş

Bu aktarım otomatik çalıştırılmaz. Eski veritabanı ve Docker veri volume'leri silinmez.
1. Her iki veritabanının yedeğini alın ve geçiş süresince başvuru yazımlarını durdurun.
2. Yeni JobService'i başlatın: boş veritabanı oluşturulur; mevcut Job veritabanına JobApplications tablosu ve indeksleri eklenir.
3. psql bağlantısı için normal PostgreSQL bağlantı ayarlarınızı kullanın. Eski başvuru veritabanına bağlanıp aşağıdaki komutu çalıştırın:

~~~sql
\copy (SELECT "Id", "CandidateId", "JobPostingId", "AppliedAt", "Status", "CreatedAt", "UpdatedAt" FROM "JobApplications") TO 'job-applications.csv' WITH (FORMAT csv, HEADER true)
~~~

4. CSV'nin bulunduğu klasörden hedef JobService veritabanına bağlanın ve scripts/import-job-applications.sql dosyasını psql -v ON_ERROR_STOP=1 -f ile çalıştırın.
5. Kaynak kayıtların ID ve içeriklerini hedefte doğrulayın. Gateway'i yeni yapılandırmayla başlatıp başvuru oluşturma/listeleme ve istatistik akışlarını kontrol edin, ardından yazımları açın.

Import aynı ID ve aynı içerik için tekrar çalıştırılabilir. Eksik ilan, desteklenmeyen durum (örneğin eski Withdrawn) veya çelişen ID varsa transaction geri alınır; kayıtlar sessizce atlanmaz/dönüştürülmez.
Aktarım tamamlanana kadar eski veritabanını yedek olarak koruyun. CSV başvuru verisi içerir; doğrulamadan sonra güvenli şekilde saklayın veya kaldırın.

## Kontroller

JobService unit ve integration test projeleri hem ilan hem başvuru testlerini içerir.
PostgreSQL entegrasyon testleri Docker gerektirir. Veri aktarımı üretim verisi üzerinde ayrıca doğrulanmalıdır.
