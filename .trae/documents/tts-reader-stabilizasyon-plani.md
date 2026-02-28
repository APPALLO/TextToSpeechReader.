## Amaç

Uygulamanın sorunsuz açılması, Material Design kaynaklarının doğru yüklenmesi ve Karaoke (okunan kelimeyi vurgulama) akışının UI thread kurallarına uygun, stabil ve akıcı çalışması.

## Kapsam

- XAML kaynak/tema yükleme hatalarını kalıcı şekilde düzeltmek
- Material Design v5 ile uyumlu stil anahtarlarını kullanmak
- TTS ilerleme event’inden gelen vurgulamayı UI thread’e güvenli taşımak
- `SpeakProgress` pozisyon bilgisini doğru property üzerinden okumak
- “app.baml duplicate key” ve dosya kilitlenmesi gibi derleme/çalıştırma tutarsızlıklarını azaltmak
- Uygulamayı çalıştırıp temel senaryoları doğrulamak

## Uygulama Adımları

### 1) Tema/Kaynak Sözlüklerini Netleştir

1. `App.xaml` içinde Material Design v5 ile uyumlu kaynakları doğrula:
   - `BundledTheme` kullanımı
   - `MaterialDesign2.Defaults.xaml` veya `MaterialDesign3.Defaults.xaml` seçimi (projedeki stil kullanımına göre)
2. `MainWindow.xaml` içinde kullanılan statik resource anahtarlarını tarayıp:
   - v5’te kaldırılmış/yenilenmiş anahtarları eşleştir (örn. Accent → Secondary).
3. Gerekirse eksik sözlükleri `App.xaml`’e ekle (örn. buton/slider/expander stilleri bu sürümde ayrı sözlüklerdeyse).

### 2) Karaoke Vurgulamasını UI Thread’e Doğru Taşı

1. `MainWindow.xaml.cs` içindeki `OnHighlightRequested` ve `OnTextLoaded` handler’larını:
   - `Dispatcher.Invoke` yerine `Dispatcher.BeginInvoke`/`InvokeAsync` kullanacak şekilde düzenle (UI’nin kilitlenmesini önlemek için).
   - `MainTextBox` erişimini tek bir UI-thread bloğu içine al.
2. Event akışını doğrula:
   - `TtsService` → `MainViewModel.HighlightRequested` → `MainWindow.OnHighlightRequested`
   - Vurgulama event’inin background thread’den gelebileceği varsayımı ile tüm UI erişimlerini dispatcher üzerinden yap.

### 3) SpeakProgress Pozisyonunu Doğru Property ile Çöz

1. `TtsService` içinde `SpeakProgressEventArgs`’tan pozisyon almak için:
   - `CharacterPosition` kullan
   - Herhangi bir `dynamic` kullanımını kaldır
2. Pozisyonun metin uzunluğu sınırları içinde kaldığını kontrol eden koruma ekle (negatif/taşma durumları).

### 4) “app.baml duplicate key” ve Dosya Kilitlenmesi İçin Sertleştirme

1. Çalışan `TextToSpeechApp.exe` süreçlerini kapat (IDE/debug host kilitleri dahil).
2. `TextToSpeechApp/bin` ve `TextToSpeechApp/obj` temizliği uygula.
3. Tekrar derle ve çalıştır:
   - `dotnet build`
   - `dotnet run --project TextToSpeechApp/TextToSpeechApp.csproj`

### 5) Doğrulama Senaryoları

1. Uygulama açılıyor mu? (XAML parse/resource hatası olmadan)
2. Dosya ekleme (pdf/docx/txt) ile metin alanı doluyor mu?
3. “Oku”:
   - TTS başlıyor mu?
   - Vurgulama metin üzerinde ilerliyor mu?
   - UI donmadan çalışıyor mu?
4. “Duraklat/Devam/Durdur”:
   - State geçişleri düzgün mü?
5. Kaydetme:
   - WAV ve MP3 kaydı başarılı mı?

## Beklenen Çıktılar

- Uygulama XAML parse exception olmadan açılır.
- MaterialDesign stil anahtarları runtime’da bulunur.
- Karaoke vurgulama cross-thread hatası vermeden çalışır.
- Derleme/çalıştırma sırasında “app.baml duplicate key” hatası tekrarlanmaz (süreç kilidi temizliği ile).

## Notlar / Varsayımlar

- MaterialDesignThemes v5.3.0 kullanılmaya devam edilecek.
- Karaoke vurgulama için `SpeakProgressEventArgs.CharacterPosition` yeterli doğrulukta kabul edilecek.
