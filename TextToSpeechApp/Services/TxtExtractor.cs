using System.IO;

namespace TextToSpeechApp.Services;

public class TxtExtractor : ITextExtractor
{
    public string ExtractText(string filePath)
    {
        return File.ReadAllText(filePath);
    }
}
