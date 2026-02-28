using System.Text.RegularExpressions;

namespace TextToSpeechApp.Services;

public class LanguageService : ILanguageService
{
    public string DetectLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "tr-TR"; // Varsayılan

        // Çok basit bir dil tahmini (Sadece örnek amaçlı)
        // Gerçek bir projede NTextCat veya Microsoft.ML kullanılmalı.
        
        // Türkçe karakterler yoğun mu?
        int trChars = Regex.Matches(text, "[ğĞüÜşŞıİöÖçÇ]").Count;
        if (trChars > 3) return "tr-TR";

        // İngilizce yaygın kelimeler
        if (Regex.IsMatch(text, @"\b(the|and|is|are|this|that)\b", RegexOptions.IgnoreCase))
            return "en-US";

        return "tr-TR"; // Varsayılan olarak Türkçe dönelim
    }
}
