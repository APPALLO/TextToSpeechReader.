# TextToSpeechApp - İyileştirme ve Test Planı

Bu plan, uygulamanın temel özelliklerinin tamamlanmasının ardından, projenin kalitesini artırmak için birim testlerin eklenmesini ve kullanıcı arayüzünün (UI) iyileştirilmesini hedefler.

## 1. Test Altyapısının Kurulması
Orijinal tasarım belgesinde belirtilen "Test Stratejisi"ni hayata geçirmek için:
- **Test Projesi:** `TextToSpeechApp.Tests` adında yeni bir xUnit test projesi oluşturulacak.
- **Bağımlılıklar:** `xUnit`, `xUnit.runner.visualstudio` ve `Moq` (servisleri mocklamak için) paketleri eklenecek.
- **Test Kapsamı:**
  - `LanguageService`: Dil algılama mantığının doğruluğu (Örn. Türkçe ve İngilizce metinlerle).
  - `PdfTextExtractor` & `WordTextExtractor`: Örnek dosyalarla metin çıkarmanın doğruluğu.
  - `FileItem`: Model durum değişimlerinin kontrolü.

## 2. Kullanıcı Arayüzü (UI) Modernizasyonu
Mevcut arayüz işlevsel ancak temel bir görünüme sahip. Daha profesyonel bir görünüm için:
- **Stiller:** `Styles.xaml` dosyası oluşturularak butonlar, listeler ve metin kutuları için ortak stiller tanımlanacak.
- **İkonlar:** Butonlara (Oynat, Durdur, Kaydet) basit metin yerine Unicode sembolleri veya vektörel ikonlar eklenecek.
- **Renk Paleti:** Göze hoş gelen, modern bir renk şeması (örn. koyu gri/mavi tonları) uygulanacak.

## 3. Dokümantasyon
- **README.md:** Projenin nasıl kurulacağı, çalıştırılacağı ve kullanılacağını anlatan detaylı bir `README.md` dosyası kök dizine eklenecek.

## 4. Uygulama Adımları
1.  `TextToSpeechApp.Tests` projesini oluştur ve solution'a ekle.
2.  Gerekli test paketlerini yükle.
3.  Servisler için birim testleri yaz.
4.  `App.xaml` içinde ResourceDictionary tanımla ve `Styles.xaml` dosyasını oluştur.
5.  `MainWindow.xaml` dosyasını yeni stilleri kullanacak şekilde güncelle.
6.  `README.md` dosyasını oluştur.
7.  Tüm testleri çalıştır ve uygulamayı son kez kontrol et.
