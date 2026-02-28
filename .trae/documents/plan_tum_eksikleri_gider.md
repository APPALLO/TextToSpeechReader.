# TextToSpeechApp - Eksikleri Giderme Planı

Bu plan, kullanıcının sağladığı tasarım belgesindeki eksik özellikleri (MP3 kaydı, Toplu İşlem, Dil Algılama, Loglama vb.) mevcut WPF uygulamasına eklemeyi hedefler.

## 1. Paket Yönetimi ve Hazırlık
- **MP3 Desteği:** `NAudio` ve `NAudio.Lame` kütüphanelerinin projeye eklenmesi.
- **Loglama:** `Serilog` ve `Serilog.Sinks.File` kütüphanelerinin eklenmesi.
- **Ayarlar:** Ayarların JSON formatında saklanması için `System.Text.Json` (zaten mevcut) kullanımı.

## 2. Servis Katmanı Güncellemeleri
### 2.1. Logger Servisi (`ILoggerService`)
- Uygulama içi olayları ve hataları kaydedecek bir servis.
- Hem dosyaya (`logs/log.txt`) hem de UI'daki log paneline yazma yeteneği.

### 2.2. Ses Servisi Güncellemesi (`ITtsService`)
- `SaveToMp3(string text, string filePath)` metodunun eklenmesi.
- Mevcut `SaveToWav` metodunun MP3 dönüşümü ile genişletilmesi (WAV üret -> MP3'e çevir -> WAV'ı sil).

### 2.3. Dil Algılama Servisi (`ILanguageService`)
- Metnin dilini tahmin etmek için basit bir servis.
- İlk aşamada karakter analizi veya basit kelime kontrolü ile (örn. "ve", "bir" -> Türkçe; "the", "and" -> İngilizce) temel algılama.

### 2.4. Ayarlar Servisi (`ISettingsService`)
- Kullanıcının son seçtiği ses, hız, ses seviyesi ve çıktı klasörü gibi ayarların kaydedilmesi ve yüklenmesi.

## 3. ViewModel Güncellemeleri (`MainViewModel`)
- **Toplu İşlem (Batch Processing):**
  - Tek bir `FilePath` yerine `ObservableCollection<FileItem>` listesi kullanımı.
  - `FileItem` sınıfı: Dosya yolu, çıkarılan metin, işlenme durumu (Bekliyor, İşleniyor, Tamamlandı, Hata) özelliklerini içerir.
- **Log Koleksiyonu:** UI'da gösterilecek loglar için bir koleksiyon.
- **Komutlar:**
  - `AddFilesCommand`: Çoklu dosya seçimi.
  - `RemoveFileCommand`: Listeden dosya silme.
  - `ProcessAllCommand`: Listedeki tüm dosyaları sırayla sese çevirme/kaydetme.

## 4. Kullanıcı Arayüzü (UI) Güncellemeleri (`MainWindow.xaml`)
- **Dosya Listesi:** Seçilen dosyaların, durumlarının ve işlem butonlarının olduğu bir `DataGrid` veya `ListView`.
- **Log Paneli:** Alt kısımda veya ayrı bir sekmede uygulama loglarını gösteren kaydırılabilir metin alanı.
- **MP3/WAV Seçimi:** Kayıt formatı için `RadioButton` veya `ComboBox`.
- **Ayarlar:** Hız ve Ses seviyesi ayarlarının hatırlanması.

## 5. Uygulama Adımları
1.  Gerekli NuGet paketlerini yükle.
2.  `FileItem` modelini oluştur.
3.  `LoggerService`'i implemente et.
4.  `LanguageService`'i implemente et (Basit versiyon).
5.  `TtsService`'i MP3 desteği ile güncelle.
6.  `MainViewModel`'i toplu işlem ve yeni servisler için refactor et.
7.  `MainWindow.xaml` arayüzünü yeni tasarıma göre güncelle.
8.  Test et ve çalıştır.
