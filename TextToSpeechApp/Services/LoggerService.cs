using Serilog;
using System;
using System.IO;

namespace TextToSpeechApp.Services;

public class LoggerService : ILoggerService
{
    public event Action<string>? OnLogReceived;

    public LoggerService()
    {
        string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        if (!Directory.Exists(logFolder))
        {
            Directory.CreateDirectory(logFolder);
        }

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(logFolder, "log-.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public void LogInformation(string message)
    {
        Log.Information(message);
        NotifyUI($"[INFO] {message}");
    }

    public void LogWarning(string message)
    {
        Log.Warning(message);
        NotifyUI($"[WARN] {message}");
    }

    public void LogError(string message, Exception? ex = null)
    {
        if (ex != null)
        {
            Log.Error(ex, message);
            NotifyUI($"[ERROR] {message} - {ex.Message}");
        }
        else
        {
            Log.Error(message);
            NotifyUI($"[ERROR] {message}");
        }
    }

    private void NotifyUI(string logEntry)
    {
        string timestampedLog = $"{DateTime.Now:HH:mm:ss} - {logEntry}";
        OnLogReceived?.Invoke(timestampedLog);
    }
}
