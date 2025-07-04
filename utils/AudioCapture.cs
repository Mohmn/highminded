using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.IO;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace highminded.utils;

public static class AudioCapture
{

    public class AudioRecorder
    {
        private WasapiLoopbackCapture capture;
        private WaveFileWriter writer;
        private string outputFilePath;
        private Action<int> visualCallback;
        public AudioRecorder(string filePath)
        {
            outputFilePath = filePath;
        }
        public void StartRecording()
        {
            capture = new WasapiLoopbackCapture();
            writer = new WaveFileWriter(outputFilePath, capture.WaveFormat);
            capture.DataAvailable += Capture_DataAvailable;
            capture.RecordingStopped += Capture_RecordingStopped;
            capture.StartRecording();
        }
        public void StopRecording()
        {
            capture.StopRecording();
        }
        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Console.WriteLine($"Received {e.BytesRecorded} bytes");
            writer.Write(e.Buffer, 0, e.BytesRecorded); //dont cram random stuff in this function unless you want delay.
        }
        private void Capture_RecordingStopped(object sender, StoppedEventArgs e)
        {
            writer.Flush();
            writer.Dispose();
            capture.Dispose();
        }

    }
    public static void start_record()
    {
        string filePath = "output.wav";
        AudioRecorder recorder = new AudioRecorder(filePath);
        recorder.StartRecording();  
    }
    public static void stop_record()
    {
        string filePath = "output.wav";
        AudioRecorder recorder = new AudioRecorder(filePath);
        recorder.StopRecording();
    }
    
} 

