using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TextToSpeechApp.Models;
using TextToSpeechApp.Services;

namespace TextToSpeechApp.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
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
    private ObservableCollection<string> _highlightColors = new() { "Blue", "Yellow", "Green", "Cyan", "Magenta", "Orange", "Red" };

    [ObservableProperty]
    private string _selectedHighlightColor = "Blue";

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

    public MainViewModel(
        ITtsService ttsService,
        IFileService fileService,
        ILoggerService loggerService,
        ILanguageService languageService,
        ISettingsService settingsService)
    {
        _localTtsService = ttsService;
        _fileService = fileService;
        _loggerService = loggerService;
        _languageService = languageService;
        _settingsService = settingsService;

        _localTtsService.SpeakProgress += (s, pos) => 
        {
            var absPos = pos + _readingOffset;
            CaretPosition = absPos;
            HighlightRequested?.Invoke(this, absPos);
        };
        
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
        var filter = "Supported Files|*.pdf;*.docx;*.txt;*.jpg;*.jpeg;*.png;*.bmp|PDF Files|*.pdf|Word Documents|*.docx|Text Files|*.txt|Image Files|*.jpg;*.jpeg;*.png;*.bmp";
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
            
            string extractedText = await Task.Run(async () => 
            {
                ITextExtractor extractor = Path.GetExtension(path).ToLower() switch
                {
                    ".pdf" => new PdfTextExtractor(),
                    ".docx" => new WordTextExtractor(),
                    ".txt" => new TxtExtractor(),
                    ".jpg" or ".jpeg" or ".png" or ".bmp" => new ImageTextExtractor(),
                    _ => throw new NotSupportedException("Desteklenmeyen dosya türü")
                };
                return await extractor.ExtractText(path);
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
        try
        {
            if (IsPlaying)
            {
                if (_localTtsService.IsSpeaking)
                {
                    _localTtsService.Pause();
                    IsPlaying = false; 
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
        catch (Exception ex)
        {
            _loggerService.LogError("Oynat/Duraklat hatası", ex);
            StatusMessage = "Hata oluştu";
        }
    }

    [RelayCommand]
    private void Pause()
    {
        try
        {
            _localTtsService.Pause();
            IsPlaying = false;
            StatusMessage = "Duraklatıldı";
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Duraklatma hatası", ex);
        }
    } 

    [RelayCommand]
    private void Resume()
    {
        try
        {
            _localTtsService.Resume();
            IsPlaying = true;
            StatusMessage = "Devam ediliyor...";
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Devam etme hatası", ex);
        }
    }

    [RelayCommand]
    private void Stop()
    {
        try
        {
            _localTtsService.Stop();
            IsPlaying = false;
            StatusMessage = "Durduruldu";
            _readingOffset = 0;
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Durdurma hatası", ex);
        }
    }

    [RelayCommand]
    private void MoveToNextSentence()
    {
        if (SelectedFile == null || string.IsNullOrEmpty(SelectedFile.ExtractedText)) return;
        
        string text = SelectedFile.ExtractedText;
        int currentPos = Math.Clamp(CaretPosition, 0, text.Length - 1);
        
        int i = currentPos;
        while (i < text.Length)
        {
            char c = text[i];
            if ((c == '.' || c == '!' || c == '?') && (i + 1 >= text.Length || char.IsWhiteSpace(text[i + 1])))
            {
                i++;
                while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
                
                if (i < text.Length)
                {
                    MoveToPositionAndRestart(i);
                    return;
                }
            }
            i++;
        }
    }

    [RelayCommand]
    private void MoveToPreviousSentence()
    {
        if (SelectedFile == null || string.IsNullOrEmpty(SelectedFile.ExtractedText)) return;
        
        string text = SelectedFile.ExtractedText;
        int currentPos = Math.Clamp(CaretPosition, 0, text.Length - 1);
        
        // Önce mevcut cümlenin başını bul
        int currentSentenceStart = FindSentenceStartBefore(text, currentPos);
        
        // Eğer zaten başındaysak veya çok yakınsak, bir önceki cümleye git
        if (currentPos - currentSentenceStart < 5 && currentSentenceStart > 0)
        {
            int prevSentenceStart = FindSentenceStartBefore(text, currentSentenceStart - 1);
            MoveToPositionAndRestart(prevSentenceStart);
        }
        else
        {
            MoveToPositionAndRestart(currentSentenceStart);
        }
    }

    private int FindSentenceStartBefore(string text, int position)
    {
        if (position <= 0) return 0;
        int i = position - 1;
        while (i > 0)
        {
             char c = text[i];
             if ((c == '.' || c == '!' || c == '?') && (i + 1 < text.Length && char.IsWhiteSpace(text[i+1])))
             {
                 int nextChar = i + 1;
                 while (nextChar < text.Length && char.IsWhiteSpace(text[nextChar])) nextChar++;
                 return nextChar;
             }
             i--;
        }
        return 0;
    }

    [RelayCommand]
    private void MoveToNextParagraph()
    {
        if (SelectedFile == null || string.IsNullOrEmpty(SelectedFile.ExtractedText)) return;
        string text = SelectedFile.ExtractedText;
        int currentPos = Math.Clamp(CaretPosition, 0, text.Length - 1);
        
        int i = text.IndexOf('\n', currentPos);
        if (i != -1)
        {
            i++;
            while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
            if (i < text.Length) MoveToPositionAndRestart(i);
        }
    }

    [RelayCommand]
    private void MoveToPreviousParagraph()
    {
        if (SelectedFile == null || string.IsNullOrEmpty(SelectedFile.ExtractedText)) return;
        string text = SelectedFile.ExtractedText;
        int currentPos = Math.Clamp(CaretPosition, 0, text.Length - 1);
        
        // Önceki satır başını bulmak için geriye doğru tara
        // LastIndexOf kullanarak
        if (currentPos > 0)
        {
            int i = text.LastIndexOf('\n', currentPos - 1);
            // Eğer şu anki satırın başındaysak, bir önceki satırı bul
            if (i != -1 && currentPos - i < 5) // Tolerans
            {
                i = text.LastIndexOf('\n', i - 1);
            }
            
            if (i != -1)
            {
                i++;
                while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
                if (i < text.Length) MoveToPositionAndRestart(i);
            }
            else
            {
                MoveToPositionAndRestart(0);
            }
        }
    }

    private void MoveToPositionAndRestart(int position)
    {
        CaretPosition = position;
        _readingOffset = position;
        
        // UI Highlight güncelle
        HighlightRequested?.Invoke(this, position);

        if (IsPlaying)
        {
            _localTtsService.Stop();
            SpeakCurrentCommand.Execute(null);
        }
    }

    public void Dispose()
    {
        _localTtsService?.Dispose();
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
                
                await Task.Run(() => 
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
