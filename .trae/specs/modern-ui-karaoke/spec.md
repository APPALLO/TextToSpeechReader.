# Modern UI ve Okuma Takibi (Karaoke) Modu Spesifikasyonu

## Neden
Kullanıcı deneyimini iyileştirmek için daha modern, şık ve animasyonlu bir arayüz ile okunan metnin takip edilmesini sağlayan (Karaoke modu) bir özelliğe ihtiyaç vardır. Bu, uygulamanın profesyonel görünmesini ve eğitim/erişilebilirlik açısından daha işlevsel olmasını sağlayacaktır.

## Neler Değişecek
- **Arayüz (UI):**
  - Standart WPF kontrolleri yerine modern stiller (Yuvarlatılmış köşeler, gölgeler, geçiş efektleri).
  - Butonlar ve paneller için yeni renk paleti ve görsel hiyerarşi.
  - İkon kullanımı (Emoji yerine Path/Vector ikonları veya daha şık font ikonlar).
- **Okuma Takibi (Karaoke):**
  - `System.Speech` motorunun `SpeakProgress` olayı dinlenecek.
  - Metin görüntüleyici `TextBox` yerine `RichTextBox`'a dönüştürülecek (Arka plan rengi ile vurgulama yapabilmek için).
  - Okunan kelime anlık olarak vurgulanacak.

## Etkiler
- **Etkilenen Dosyalar:**
  - `MainWindow.xaml`: UI tamamen yenilenecek.
  - `Styles.xaml`: Yeni stiller eklenecek.
  - `TtsService.cs`: İlerleme olayları (event) eklenecek.
  - `MainViewModel.cs`: İlerleme olaylarını UI'a taşıyacak mantık eklenecek.
  - `FileItem.cs`: Modelde değişiklik gerekebilir (veya VM üzerinde yönetilir).

## EKLENEN Gereksinimler
### Gereksinim: Modern Arayüz
Sistem, modern tasarım prensiplerine (Flat/Material benzeri) uygun, göze hoş gelen ve animasyonlu bir arayüze sahip olmalıdır.

#### Senaryo: Uygulama Açılışı
- **WHEN** kullanıcı uygulamayı açtığında
- **THEN** modern, temiz ve karanlık moda tam uyumlu bir arayüz görmelidir.

### Gereksinim: Kelime Vurgulama (Karaoke)
Sistem, seslendirme sırasında o an okunan kelimeyi metin üzerinde vurgulamalıdır.

#### Senaryo: Okuma Sırasında
- **WHEN** kullanıcı "Oku" butonuna bastığında
- **THEN** metin seslendirilirken, okunan kelimenin arka planı veya rengi değişerek kullanıcının takip etmesi sağlanmalıdır.

## DEĞİŞTİRİLEN Gereksinimler
### Gereksinim: Metin Görüntüleme
`TextBox` yerine `RichTextBox` kullanılarak zengin metin biçimlendirme ve vurgulama yeteneği kazandırılacaktır.
