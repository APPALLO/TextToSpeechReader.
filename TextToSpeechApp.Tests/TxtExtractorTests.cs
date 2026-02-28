using TextToSpeechApp.Services;

namespace TextToSpeechApp.Tests;

public class TxtExtractorTests
{
    [Fact]
    public async Task ExtractText_ShouldReturnFileContent()
    {
        // Arrange
        var extractor = new TxtExtractor();
        var tempFile = Path.GetTempFileName();
        var content = "Merhaba Dünya";
        await File.WriteAllTextAsync(tempFile, content);

        try
        {
            // Act
            var result = await extractor.ExtractText(tempFile);

            // Assert
            Assert.Equal(content, result);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
