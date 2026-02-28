# Metinden Sese Dönüştürücü (TextToSpeechReader)

Bu proje, PDF, Word (.docx) ve Metin (.txt) dosyalarını okuyup sese dönüştüren, metinleri analiz eden ve ses dosyası (MP3/WAV) olarak kaydeden bir Windows masaüstü uygulamasıdır.

## Özellikler

- **Dosya Desteği:** PDF, Word (.docx) ve TXT dosyalarını okuyabilir.
- **Toplu İşlem:** Birden fazla dosyayı listeye ekleyip yönetebilirsiniz.
- **Dil Algılama:** Metin içeriğine göre otomatik dil tahmini (Türkçe/İngilizce) yapar.
- **Ses Ayarları:** Yüklü sesler arasından seçim, okuma hızı ve ses seviyesi ayarı.
- **Kayıt:** Metinleri **MP3** veya **WAV** formatında bilgisayarınıza kaydedebilirsiniz.
- **Loglama:** Tüm işlemler ve hatalar uygulama içinde ve `logs` klasöründe kaydedilir.
- **Modern Arayüz:** WPF ile geliştirilmiş, kullanıcı dostu karanlık mod arayüzü.

## Kurulum ve Çalıştırma

### Gereksinimler
- .NET 9.0 SDK
- Windows İşletim Sistemi (TTS motoru için)

### Çalıştırma Adımları
1. Proje dizininde terminali açın.
2. Bağımlılıkları yükleyin ve derleyin:
   ```powershell
   dotnet build
   ```
3. Uygulamayı başlatın:
   ```powershell
   dotnet run --project TextToSpeechApp/TextToSpeechApp.csproj
   ```

## Testler

Proje, temel servislerin doğruluğunu kontrol eden birim testleri içerir. Testleri çalıştırmak için:

```powershell
dotnet test
```

## Kullanılan Teknolojiler

- **WPF (.NET 9.0):** Kullanıcı arayüzü.
- **CommunityToolkit.Mvvm:** MVVM mimarisi.
- **PdfPig:** PDF metin çıkarma.
- **OpenXML SDK:** Word metin çıkarma.
- **System.Speech:** Metin okuma (TTS).
- **NAudio & NAudio.Lame:** MP3 dönüştürme ve kaydetme.
- **Serilog:** Loglama altyapısı.
- **xUnit & Moq:** Test framework'ü.

## Geliştirici Notları

- Loglar `TextToSpeechApp/bin/Debug/net9.0-windows/logs` klasöründe günlük olarak tutulur.
- MP3 dönüşümü için `libmp3lame` kütüphanesi kullanılır (NuGet paketi ile otomatik gelir).
