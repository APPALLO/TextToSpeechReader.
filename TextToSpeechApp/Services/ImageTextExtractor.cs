using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;

namespace TextToSpeechApp.Services;

public class ImageTextExtractor : ITextExtractor
{
    public async Task<string> ExtractText(string filePath)
    {
        try
        {
            // Dosyayı WinRT StorageFile olarak al
            // Not: filePath mutlak yol olmalı
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                // BitmapDecoder oluştur
                var decoder = await BitmapDecoder.CreateAsync(stream);
                
                // SoftwareBitmap'i OcrEngine'in desteklediği formatta (Bgra8) al
                // OcrEngine genellikle Bgra8 veya Gray8 bekler
                using (var softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied))
                {
                    // OcrEngine başlat
                    // Önce kullanıcının profil dillerini dener (örn. Türkçe, İngilizce)
                    var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                    
                    if (ocrEngine == null)
                    {
                        // Eğer profil dilleri desteklenmiyorsa, sistemdeki ilk kullanılabilir dili dener
                        if (OcrEngine.AvailableRecognizerLanguages.Count > 0)
                        {
                            ocrEngine = OcrEngine.TryCreateFromLanguage(OcrEngine.AvailableRecognizerLanguages[0]);
                        }
                    }
                    
                    if (ocrEngine == null)
                    {
                        throw new InvalidOperationException("OCR motoru başlatılamadı. Lütfen Windows Ayarları > Zaman ve Dil > Dil seçeneklerinden 'Optik karakter tanıma' özelliğinin yüklü olduğundan emin olun.");
                    }

                    // Metni tanı
                    var result = await ocrEngine.RecognizeAsync(softwareBitmap);
                    
                    // Sonucu döndür
                    // result.Text tüm metni verir, ancak satırları korumak istersek Lines üzerinde dönebiliriz
                    // result.Text genellikle satırları birleştirir veya boşlukla ayırır.
                    // Daha düzenli bir çıktı için Lines kullanalım.
                    
                    var sb = new StringBuilder();
                    foreach (var line in result.Lines)
                    {
                        sb.AppendLine(line.Text);
                    }
                    
                    return sb.ToString().Trim();
                }
            }
        }
        catch (Exception ex)
        {
            // Hata detayını yukarı fırlat, MainViewModel yakalasın
            throw new Exception($"OCR işlemi başarısız: {ex.Message}", ex);
        }
    }
}