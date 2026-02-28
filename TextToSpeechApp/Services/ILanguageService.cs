namespace TextToSpeechApp.Services;

public interface ILanguageService
{
    string DetectLanguage(string text);
}
