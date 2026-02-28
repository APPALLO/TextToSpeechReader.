using System;
using System.IO;
using System.Text.Json;

namespace TextToSpeechApp.Services;

public class SettingsService : ISettingsService
{
    private const string SettingsFile = "settings.json";
    
    // No specific settings for now

    private class SettingsData
    {
        // Empty
    }

    public void Save()
    {
        var data = new SettingsData
        {
        };
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(SettingsFile, json);
    }

    public void Load()
    {
        if (File.Exists(SettingsFile))
        {
            try
            {
                var json = File.ReadAllText(SettingsFile);
                var data = JsonSerializer.Deserialize<SettingsData>(json);
                if (data != null)
                {
                    // Load settings
                }
            }
            catch
            {
                // Ignore errors, use defaults
            }
        }
    }
}
