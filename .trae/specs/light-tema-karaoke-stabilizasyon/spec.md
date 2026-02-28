# Light Tema + Karaoke Stabilizasyon Spec

## Why
Uygulama çalışma anında XAML kaynak/stil hataları ve Karaoke (vurgulama) sırasında thread erişim hataları ile çökebiliyor. Light tema ile stabil ve tutarlı bir açılış/okuma deneyimi hedefleniyor.

## What Changes
- Material Design v5 uyumlu kaynak sözlükleri ile uygulama kaynaklarını standardize etmek.
- Dark mod/dark tema etkisi oluşturan özel stilleri kaldırmak ve Light tema ile devam etmek.
- Karaoke vurgulama akışını UI thread üzerinde güvenli ve akıcı çalışacak şekilde düzenlemek.
- TTS ilerleme pozisyonunu doğru property üzerinden almak (SpeakProgressEventArgs).
- Derleme/çalıştırma sırasında oluşabilen kilitlenme/çift BAML anahtarı gibi tutarsızlıkların etkisini azaltmak.

## Impact
- Affected specs: Tema yükleme, UI stilleri, Karaoke vurgulama, TTS event akışı
- Affected code:
  - App.xaml (uygulama kaynak sözlükleri)
  - MainWindow.xaml (stil anahtarları)
  - MainWindow.xaml.cs (Karaoke vurgulama/UI thread)
  - Services/TtsService.cs (SpeakProgress pozisyonu)

## ADDED Requirements
### Requirement: Light Tema (Tek Tema)
Sistem yalnızca Light tema ile çalışacaktır.

#### Scenario: Uygulama açılışı
- **WHEN** kullanıcı uygulamayı başlatır
- **THEN** arayüz Light tema renkleri ile açılır
- **AND** XAML parse/resource hatası oluşmaz

### Requirement: Stabil Karaoke Vurgulama
Sistem, okuma sırasında metinde konuşulan kelimeyi vurgulamalıdır.

#### Scenario: Okuma sırasında vurgulama
- **WHEN** kullanıcı “Oku” butonuna basar
- **THEN** TTS konuşma başladıkça vurgulama metin üzerinde ilerler
- **AND** UI thread hatası oluşmaz
- **AND** arayüz donmadan çalışır

## MODIFIED Requirements
### Requirement: SpeakProgress Pozisyon Erişimi
Sistem, SpeakProgress event’inden alınan konumu `SpeakProgressEventArgs.CharacterPosition` üzerinden hesaplayacaktır.

## REMOVED Requirements
### Requirement: Dark Mod Özel Stilleri
**Reason**: Light tema ile devam edilmesi isteniyor ve Dark mod stilleri Material Design temasıyla çakışabiliyor.
**Migration**: Dark mod’a özel `Styles.xaml` gibi sözlüklerin uygulama kaynaklarından çıkarılması; yalnızca Material Design Light teması kullanımı.

