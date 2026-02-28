using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TextToSpeechApp.Models;
using TextToSpeechApp.Services;

namespace TextToSpeechApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITtsService _localTtsService;
    private readonly IFileService _fileService;
    private readonly ILoggerService _loggerService;
    private readonly ILanguageService _languageService;
    private readonly ISettingsService _settingsService;
    
    [ObservableProperty]
    private ObservableCollection<FileItem> _files;

    [ObservableProperty]
    private FileItem? _selectedFile;

    [ObservableProperty]
    private ObservableCollection<string> _logs;

    [ObservableProperty]
    private ObservableCollection<string> _voices = new();
    
    [ObservableProperty]
    private string? _selectedVoice;
    
    [ObservableProperty]
    private int _rate = 0; // -10 to 10
    
    [ObservableProperty]
    private int _volume = 100; // 0 to 100
    
    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private bool _saveAsMp3; 

    [ObservableProperty]
    private string _statusMessage = "Hazır";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TogglePlayPauseCommand))]
    private bool _isPlaying;

    [ObservableProperty]
    private int _caretPosition;

    private int _readingOffset = 0;

    // Eventler
    public event EventHandler<int>? HighlightRequested;
    public event EventHandler<string>? TextLoaded;

    public MainViewModel()
    {
        _localTtsService = new TtsService();
        _localTtsService.SpeakProgress += (s, pos) => HighlightRequested?.Invoke(this, pos + _readingOffset);
        
        _fileService = new FileService();
        _loggerService = new LoggerService();
        _languageService = new LanguageService();
        _settingsService = new SettingsService();
        
        Files = new ObservableCollection<FileItem>();
        Logs = new ObservableCollection<string>();
        
        _loggerService.OnLogReceived += (log) => 
        {
            Application.Current.Dispatcher.Invoke(() => Logs.Insert(0, log));
        };

        // Ayarları yükle
        _settingsService.Load();
        
        LoadVoices();
        _loggerService.LogInformation("Uygulama başlatıldı.");
    }

    private void LoadVoices()
    {
        try 
        {
            var voices = _localTtsService.GetInstalledVoices();
            Voices = new ObservableCollection<string>(voices);
            SelectedVoice = Voices.FirstOrDefault();
            
            if (SelectedVoice != null)
            {
                _localTtsService.SetVoice(SelectedVoice);
            }
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Sesler yüklenirken hata oluştu.", ex);
        }
    }

    partial void OnSelectedVoiceChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _localTtsService.SetVoice(value);
            _loggerService.LogInformation($"Ses değiştirildi: {value}");
        }
    }

    partial void OnRateChanged(int value)
    {
        _localTtsService.SetRate(value);
    } 
    
    partial void OnVolumeChanged(int value)
    {
        _localTtsService.SetVolume(value);
    }

    [RelayCommand]
    private async Task AddFiles()
    {
        var filter = "Supported Files|*.pdf;*.docx;*.txt|PDF Files|*.pdf|Word Documents|*.docx|Text Files|*.txt";
        var path = _fileService.OpenFile(filter);
        if (!string.IsNullOrEmpty(path))
        {
            await ProcessFile(path);
        }
    }

    private async Task ProcessFile(string path)
    {
        var fileItem = new FileItem { FilePath = path, Status = "Yükleniyor..." };
        Files.Add(fileItem);
        
        try
        {
            _loggerService.LogInformation($"Dosya okunuyor: {path}");
            
            string extractedText = await Task.Run(() => 
            {
                ITextExtractor extractor = Path.GetExtension(path).ToLower() switch
                {
                    ".pdf" => new PdfTextExtractor(),
                    ".docx" => new WordTextExtractor(),
                    ".txt" => new TxtExtractor(),
                    _ => throw new NotSupportedException("Desteklenmeyen dosya türü")
                };
                return extractor.ExtractText(path);
            });

            fileItem.ExtractedText = extractedText;
            fileItem.Status = "Hazır";
            
            // TextLoaded eventini tetikle
            TextLoaded?.Invoke(this, extractedText);
            
            var lang = _languageService.DetectLanguage(extractedText);
            fileItem.Message = $"Dil: {lang} - Karakter: {extractedText.Length}";
            _loggerService.LogInformation($"Dosya hazır: {path} ({lang})");
        }
        catch (Exception ex)
        {
            fileItem.Status = "Hata";
            fileItem.Message = ex.Message;
            _loggerService.LogError($"Dosya okuma hatası: {path}", ex);
        }
    }

    [RelayCommand]
    private void RemoveFile()
    {
        if (SelectedFile != null)
        {
            Files.Remove(SelectedFile);
            _loggerService.LogInformation("Dosya listeden kaldırıldı.");
        }
    }

    [RelayCommand]
    private async Task SpeakCurrent()
    {
        if (SelectedFile != null && !string.IsNullOrEmpty(SelectedFile.ExtractedText))
        {
            try
            {
                StatusMessage = "Seslendiriliyor...";
                _loggerService.LogInformation("Seslendirme başlatıldı.");
                IsPlaying = true;

                string textToSpeak = SelectedFile.ExtractedText;
                // Eğer imleç bir yerde ise ve metin sonuna gelmediyse oradan başla
                if (CaretPosition > 0 && CaretPosition < textToSpeak.Length - 1)
                {
                    textToSpeak = textToSpeak.Substring(CaretPosition);
                    _readingOffset = CaretPosition;
                }
                else
                {
                    _readingOffset = 0;
                }

                await Task.Run(() => _localTtsService.Speak(textToSpeak));
                IsPlaying = false;
                StatusMessage = "Tamamlandı";
                _readingOffset = 0; // Sıfırla
            }
            catch (Exception ex)
            {
                IsPlaying = false;
                StatusMessage = "Hata oluştu";
                _loggerService.LogError("Seslendirme hatası", ex);
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private void TogglePlayPause()
    {
        if (IsPlaying)
        {
            if (_localTtsService.IsSpeaking)
            {
                _localTtsService.Pause();
                IsPlaying = false; // Aslında Pause durumunda da Playing true kalabilir ama UI için false yapıyoruz
                StatusMessage = "Duraklatıldı";
            }
        }
        else
        {
            if (SelectedFile != null && !string.IsNullOrEmpty(SelectedFile.ExtractedText))
            {
                if (_localTtsService.IsPaused)
                {
                    _localTtsService.Resume();
                    IsPlaying = true;
                    StatusMessage = "Devam ediliyor...";
                }
                else
                {
                    SpeakCurrentCommand.Execute(null);
                }
            }
        }
    }

    [RelayCommand]
    private void Pause()
    {
        _localTtsService.Pause();
        IsPlaying = false;
        StatusMessage = "Duraklatıldı";
    } 

    [RelayCommand]
    private void Resume()
    {
        _localTtsService.Resume();
        IsPlaying = true;
        StatusMessage = "Devam ediliyor...";
    }

    [RelayCommand]
    private void Stop()
    {
        _localTtsService.Stop();
        IsPlaying = false;
        StatusMessage = "Durduruldu";
        _readingOffset = 0;
    }

    [RelayCommand]
    private async Task SaveSelected()
    {
        if (SelectedFile == null || string.IsNullOrEmpty(SelectedFile.ExtractedText)) return;

        string ext = SaveAsMp3 ? "mp3" : "wav";
        var filter = $"{ext.ToUpper()} Files|*.{ext}";
        var path = _fileService.SaveFile(filter);
        
        if (!string.IsNullOrEmpty(path))
        {
            IsProcessing = true;
            SelectedFile.Status = "Kaydediliyor...";
            try
            {
                _loggerService.LogInformation($"Kayıt başladı: {path}");
                
                await Task.Run(async () => 
                {
                    if (SaveAsMp3)
                        _localTtsService.SaveToMp3(SelectedFile.ExtractedText, path);
                    else
                        _localTtsService.SaveToWav(SelectedFile.ExtractedText, path);
                });
                
                SelectedFile.Status = "Tamamlandı";
                _loggerService.LogInformation("Dosya başarıyla kaydedildi.");
                MessageBox.Show("Dosya başarıyla kaydedildi!");
            }
            catch (Exception ex)
            {
                SelectedFile.Status = "Hata";
                _loggerService.LogError("Kayıt hatası", ex);
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
