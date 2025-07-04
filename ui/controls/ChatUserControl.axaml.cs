using Avalonia.Controls;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input;
using highminded.utils;
using OpenAI.Chat;
using Markdig;
using Markdown.ColorCode;
using System.Net.Http;

namespace highminded.ui.controls;

public partial class ChatUserControl : UserControl
{
    private readonly MarkdownPipeline _pipeline = null!;
    private AudioCapture.AudioRecorder? _audioRecorder;
    private const string AudioFilePath = "output.wav";

    public ChatUserControl()
    {
        InitializeComponent();
        _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode().Build();
    }

    public void StartRecord()
    {
        _audioRecorder = new AudioCapture.AudioRecorder(AudioFilePath);
        _audioRecorder.StartRecording();
    }

    public void StopRecord()
    {
        if (_audioRecorder != null)
        {
            _audioRecorder.StopRecording();
            OnRecordingStopped(null, EventArgs.Empty);
        }
    }

    private void OnRecordingStopped(object? sender, EventArgs e)
    {
        _audioRecorder = null;
        SendAudio();
    }

    public async void SendAudio()
    {
        try
        {
            if (!File.Exists("output.wav"))
                throw new Exception("Audio file not found");


            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = "output.wav";
            var destPath = Path.Combine(Environment.CurrentDirectory, fileName);
            File.Copy("output.wav", destPath, true);

            await using Stream audioStream = File.OpenRead("output.wav");
            var audioBytes = await BinaryData.FromStreamAsync(audioStream);

            List<ChatMessage> messages =
            [
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(InMemoryDb.Obj.SettingsManager.Settings.AudioPrompt),
                    ChatMessageContentPart.CreateInputAudioPart(audioBytes, "audio/wav")
                )
            ];

            await ProcessChatStreamAsync(messages);
        }
        catch (Exception err)
        {
            ResultBlock.Text = err.Message;
        }
    }

    public async void SendScreenshot()
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"screenshot_{timestamp}.png";
            var filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            var screenshot = await ScreenCapture.CaptureScreenAsync(filePath);
            if (!screenshot) throw new Exception("Failed to capture screenshot");

            await using Stream imageStream = File.OpenRead(filePath);
            var imageBytes = await BinaryData.FromStreamAsync(imageStream);

            List<ChatMessage> messages =
            [
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(InMemoryDb.Obj.SettingsManager.Settings.ScreenshotPrompt),
                    ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")
                )
            ];

            await ProcessChatStreamAsync(messages);
        }
        catch (Exception err)
        {
            ResultBlock.Text = err.Message;
        }
    }

    private async void PromptBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key != Key.Enter) return;

            var prompt = PromptBox.Text;
            if (prompt is null) return;
            PromptBox.Clear();

            await ProcessChatStreamAsync(prompt);
        }
        catch (Exception err)
        {
            ResultBlock.Text = err.Message;
        }
    }

    private async Task ProcessChatStreamAsync(object promptOrMessages)
    {
        AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = promptOrMessages switch
        {
            string prompt => InMemoryDb.Obj.ChatClient.CompleteChatStreamingAsync(prompt),
            IEnumerable<ChatMessage> messages => InMemoryDb.Obj.ChatClient.CompleteChatStreamingAsync(messages),
            _ => throw new ArgumentException("Invalid input type", nameof(promptOrMessages))
        };

        var responseBuilder = new StringBuilder();

        await foreach (var completionUpdate in completionUpdates)
        {
            if (completionUpdate.ContentUpdate.Count <= 0) continue;

            var token = completionUpdate.ContentUpdate[0].Text;
            responseBuilder.Append(token);

            var html = Markdig.Markdown.ToHtml(responseBuilder.ToString(), _pipeline);
            ResultBlock.Text = html;
        }
    }
}