namespace TextToSpeechApp.Services;

public interface ILoggerService
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? ex = null);
    
    // UI'da göstermek için olay
    event Action<string> OnLogReceived;
}
