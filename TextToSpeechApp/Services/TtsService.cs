using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using NAudio.Wave;
using NAudio.Lame;

namespace TextToSpeechApp.Services;

public class TtsService : ITtsService, IDisposable
{
    private readonly SpeechSynthesizer _synthesizer;

    public event EventHandler<int>? SpeakProgress;

    public TtsService()
    {
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SetOutputToDefaultAudioDevice();
        _synthesizer.SpeakProgress += (object? s, SpeakProgressEventArgs e) => SpeakProgress?.Invoke(this, e.CharacterPosition);
    }

    public void Speak(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        _synthesizer.SpeakAsync(text);
    }

    public void Pause()
    {
        if (_synthesizer.State == SynthesizerState.Speaking)
            _synthesizer.Pause();
    }

    public void Resume()
    {
        if (_synthesizer.State == SynthesizerState.Paused)
            _synthesizer.Resume();
    }

    public void Stop()
    {
        _synthesizer.SpeakAsyncCancelAll();
    }

    public void SaveToWav(string text, string filePath)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        _synthesizer.SetOutputToWaveFile(filePath);
        _synthesizer.Speak(text);
        _synthesizer.SetOutputToDefaultAudioDevice();
    }

    public void SaveToMp3(string text, string filePath)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        string tempWav = Path.ChangeExtension(filePath, ".temp.wav");
        try
        {
            // Önce WAV olarak kaydet
            SaveToWav(text, tempWav);

            // WAV -> MP3 Dönüştür
            using (var reader = new AudioFileReader(tempWav))
            using (var writer = new LameMP3FileWriter(filePath, reader.WaveFormat, LAMEPreset.STANDARD))
            {
                reader.CopyTo(writer);
            }
        }
        finally
        {
            // Geçici WAV dosyasını temizle
            if (File.Exists(tempWav))
            {
                File.Delete(tempWav);
            }
        }
    }

    public void SetRate(int rate)
    {
        _synthesizer.Rate = Math.Clamp(rate, -10, 10);
    }

    public void SetVolume(int volume)
    {
        _synthesizer.Volume = Math.Clamp(volume, 0, 100);
    }

    public void SetVoice(string voiceName)
    {
        _synthesizer.SelectVoice(voiceName);
    }

    public IEnumerable<string> GetInstalledVoices()
    {
        return _synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo.Name);
    }

    public void Dispose()
    {
        _synthesizer.Dispose();
    }
}
