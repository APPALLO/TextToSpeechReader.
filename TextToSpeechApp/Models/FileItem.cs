using CommunityToolkit.Mvvm.ComponentModel;

namespace TextToSpeechApp.Models;

public partial class FileItem : ObservableObject
{
    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _extractedText = string.Empty;

    [ObservableProperty]
    private string _status = "Bekliyor"; // Pending, Processing, Completed, Error

    [ObservableProperty]
    private string _message = string.Empty;

    public string FileName => System.IO.Path.GetFileName(FilePath);
}
