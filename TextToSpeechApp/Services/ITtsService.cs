using System;
using System.Collections.Generic;

namespace TextToSpeechApp.Services;

public interface ITtsService
{
    event EventHandler<int> SpeakProgress;
    void Speak(string text);
    void Pause();
    void Resume();
    void Stop();
    void SaveToWav(string text, string filePath);
    void SetRate(int rate);
    void SetVolume(int volume);
    void SetVoice(string voiceName);
    IEnumerable<string> GetInstalledVoices();
    void SaveToMp3(string text, string filePath);
}
