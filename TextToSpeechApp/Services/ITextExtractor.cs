namespace TextToSpeechApp.Services;

public interface ITextExtractor
{
    Task<string> ExtractText(string filePath);
}