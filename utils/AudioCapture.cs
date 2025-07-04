using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace highminded.utils;

public class AudioCapture
{
    private WasapiLoopbackCapture capture;
    private WaveFileWriter writer;
    private string outputFilePath;

    public AudioCapture()
    {
        capture = new WasapiLoopbackCapture();
    }

    public void StartRecording(string outPath)
    {
        writer = new WaveFileWriter(outPath, capture.WaveFormat);
        capture.DataAvailable += Capture_DataAvailable;
        capture.RecordingStopped += Capture_RecordingStopped;
        capture.StartRecording();
    }

    public void StopRecording()
    {
        capture.StopRecording();
    }

    public bool IsRecording()
    {
        if (capture.CaptureState == CaptureState.Capturing || capture.CaptureState == CaptureState.Starting)
        {
            return true;
        }

        return false;
    }

    private void Capture_DataAvailable(object sender, WaveInEventArgs e)
    {
        writer.Write(e.Buffer, 0, e.BytesRecorded);
    }

    private void Capture_RecordingStopped(object sender, StoppedEventArgs e)
    {
        writer.Flush();
        writer.Dispose();
        capture.Dispose();
    }
}