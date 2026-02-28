# Tasks

- [ ] Task 1: Material Design kaynaklarını Light tema için standardize et
  - [ ] SubTask 1.1: App.xaml merged dictionaries’te Light tema yapılandırmasını doğrula
  - [ ] SubTask 1.2: Dark moda zorlayan özel resource dictionary’leri uygulama kaynaklarından çıkar
  - [ ] SubTask 1.3: Eksik/yanlış stil anahtarlarını (StaticResource) MaterialDesign v5 karşılıklarıyla değiştir

- [ ] Task 2: TTS SpeakProgress pozisyonunu doğru property ile düzelt
  - [ ] SubTask 2.1: SpeakProgressEventArgs için CharacterPosition kullanımını uygula
  - [ ] SubTask 2.2: Sınır kontrolleri ekle (negatif/taşma durumları)

- [ ] Task 3: Karaoke vurgulamayı UI thread’de akıcı hale getir
  - [ ] SubTask 3.1: Vurgulama ve metin yükleme işlemlerini Dispatcher üzerinden asenkron çalıştır
  - [ ] SubTask 3.2: Gereksiz tekrar seçimi engelle (aynı kelimeyi tekrar tekrar seçme)
  - [ ] SubTask 3.3: UI donmalarını azaltmak için uygun DispatcherPriority kullan

- [ ] Task 4: Derleme/çalıştırma doğrulaması yap
  - [ ] SubTask 4.1: clean/build/test çalıştır ve çıktıyı doğrula
  - [ ] SubTask 4.2: Uygulamayı çalıştır ve temel senaryoları doğrula (açılış, dosya yükleme, oku/duraklat/devam/durdur, kaydet)

# Task Dependencies
- Task 3 depends on Task 2
- Task 4 depends on Task 1, Task 2, Task 3

