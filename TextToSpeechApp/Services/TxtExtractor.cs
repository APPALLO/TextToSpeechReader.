using System.IO;

namespace TextToSpeechApp.Services;

public class TxtExtractor : ITextExtractor
{
    public async Task<string> ExtractText(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }
}