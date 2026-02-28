# Görevler (Tasks)

- [ ] Task 0: Bağımlılıkları Yükle
  - [ ] SubTask 0.1: `MaterialDesignThemes` NuGet paketini projeye ekle.
  - [ ] SubTask 0.2: `App.xaml` içinde Material Design kaynaklarını tanımla.

- [ ] Task 1: TTS Servisini Güncelleme
  - [ ] SubTask 1.1: `ITtsService` arayüzüne `SpeakProgress` olayı (event) ekle.
  - [ ] SubTask 1.2: `TtsService` sınıfında `SpeechSynthesizer.SpeakProgress` olayını dinle ve arayüze ilet.

- [ ] Task 2: RichTextBox Entegrasyonu
  - [ ] SubTask 2.1: `MainWindow.xaml` içindeki `TextBox`'ı `RichTextBox` ile değiştir.
  - [ ] SubTask 2.2: `MainViewModel` içinde metni `RichTextBox`'a yükleyen ve vurgulama yapan mantığı ekle. (Not: MVVM'de RichTextBox doğrudan bağlanamaz, bir Behavior veya Code-Behind yardımı gerekebilir, şimdilik Code-Behind ile basit bir çözüm veya `FlowDocument` binding kullanılacak).
  - [ ] SubTask 2.3: Okuma sırasında gelen kelime pozisyonuna göre `RichTextBox` içinde ilgili aralığı seçip arka plan rengini değiştir.

- [ ] Task 3: Modern UI Tasarımı (Material Design)
  - [ ] SubTask 3.1: `MainWindow.xaml` ana yapısını Material Design kartları (Card) ve renkleri ile güncelle.
  - [ ] SubTask 3.2: Butonları `MaterialDesignRaisedButton` veya ikonlu butonlar ile değiştir.
  - [ ] SubTask 3.3: Slider ve ComboBox'ları Material stilleriyle güncelle.

# Bağımlılıklar
- Task 2, Task 1'e bağımlıdır.
- Task 3, Task 0'a bağımlıdır.
