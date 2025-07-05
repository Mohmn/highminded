using System.IO;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;

namespace highminded.utils;

public class AudioCapture
{
    private readonly MiniAudioEngine _audioEngine = new(48000, Capability.Loopback);
    private Stream? _fileStream;
    private Recorder? _recorder;

    public void StartRecording(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        _recorder = new Recorder(_fileStream, sampleRate: 48000);
        _recorder.StartRecording();
    }

    public void StopRecording()
    {
        _recorder?.StopRecording();
        _recorder?.Dispose();
        _fileStream?.Dispose();
    }
}