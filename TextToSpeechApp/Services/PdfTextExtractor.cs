using UglyToad.PdfPig;
using System.Text;

namespace TextToSpeechApp.Services;

public class PdfTextExtractor : ITextExtractor
{
    public Task<string> ExtractText(string filePath)
    {
        using var document = PdfDocument.Open(filePath);
        var sb = new StringBuilder();
        foreach (var page in document.GetPages())
        {
            // Satır sonlarını boşlukla değiştirerek akıcılığı sağla, ancak paragrafları korumaya çalış
            // Basit yaklaşım: Her sayfa sonuna yeni satır ekle, sayfa içi metni temizle
            string pageText = page.Text.Replace("\r\n", " ").Replace("\n", " ").Replace("  ", " ");
            sb.AppendLine(pageText);
        }
        return Task.FromResult(sb.ToString().Trim());
    }
}