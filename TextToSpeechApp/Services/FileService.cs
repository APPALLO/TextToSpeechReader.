using Microsoft.Win32;

namespace TextToSpeechApp.Services;

public class FileService : IFileService
{
    public string? OpenFile(string filter)
    {
        var dialog = new OpenFileDialog { Filter = filter };
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }
        return null;
    }

    public string? SaveFile(string filter)
    {
        var dialog = new SaveFileDialog { Filter = filter };
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }
        return null;
    }
}
