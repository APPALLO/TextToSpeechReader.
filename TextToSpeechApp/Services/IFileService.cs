namespace TextToSpeechApp.Services;

public interface IFileService
{
    string? OpenFile(string filter);
    string? SaveFile(string filter);
}
