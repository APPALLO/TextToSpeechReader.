using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace TextToSpeechApp.Services;

public class WordTextExtractor : ITextExtractor
{
    public Task<string> ExtractText(string filePath)
    {
        using var document = WordprocessingDocument.Open(filePath, false);
        var sb = new StringBuilder();
        var body = document.MainDocumentPart?.Document?.Body;
        if (body != null)
        {
            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                if (!string.IsNullOrWhiteSpace(paragraph.InnerText))
                {
                    sb.AppendLine(paragraph.InnerText.Trim());
                }
            }
        }
        return Task.FromResult(sb.ToString().Trim());
    }
}