using TextToSpeechApp.Services;

namespace TextToSpeechApp.Tests;

public class LanguageServiceTests
{
    private readonly ILanguageService _languageService;

    public LanguageServiceTests()
    {
        _languageService = new LanguageService();
    }

    [Theory]
    [InlineData("Bu bir Türkçe metindir.", "tr-TR")]
    [InlineData("Merhaba dünya, nasılsın?", "tr-TR")]
    [InlineData("Çalışmak başarının anahtarıdır.", "tr-TR")]
    public void DetectLanguage_ShouldReturnTurkish_ForTurkishText(string text, string expected)
    {
        var result = _languageService.DetectLanguage(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("This is an English text.", "en-US")]
    [InlineData("Hello world, how are you?", "en-US")]
    [InlineData("The quick brown fox jumps over the lazy dog.", "en-US")]
    public void DetectLanguage_ShouldReturnEnglish_ForEnglishText(string text, string expected)
    {
        var result = _languageService.DetectLanguage(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DetectLanguage_ShouldReturnDefault_ForEmptyText()
    {
        var result = _languageService.DetectLanguage("");
        Assert.Equal("tr-TR", result);
    }
}
