using Avalonia.Controls;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input;
using highminded.utils;
using OpenAI.Chat;
using Markdig;
using Markdown.ColorCode;

namespace highminded.ui.controls;

public partial class ChatUserControl : UserControl
{
    private readonly MarkdownPipeline _pipeline = null!;
    private AudioCapture _audioCapture = null!;

    public ChatUserControl()
    {
        InitializeComponent();
        _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode().Build();
        _audioCapture = new AudioCapture();
    }

    public void StartRecord()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"audio_{timestamp}.wav";
        var dirPath = Path.Combine(Environment.CurrentDirectory, "audio");
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        _audioCapture.StartRecording(filePath);
    }

    public void StopRecord()
    {
        if (_audioCapture != null)
        {
            _audioCapture.StopRecording();
            SendAudio();
        }
    }

    public async void SendAudio()
    {
        try
        {
            var dirPath = Path.Combine(Environment.CurrentDirectory, "audio");
            var latestAudio = new DirectoryInfo(dirPath).GetFiles().OrderByDescending(f => f.LastWriteTime).First(); 
            var filePath = Path.Combine(dirPath, latestAudio.Name);
            var audioFileBytes = await File.ReadAllBytesAsync(filePath);
            var audioBytes = BinaryData.FromBytes(audioFileBytes);

            List<ChatMessage> messages =
            [
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(InMemoryDb.Obj.SettingsManager.Settings.AudioPrompt),
                    ChatMessageContentPart.CreateInputAudioPart(audioBytes, ChatInputAudioFormat.Wav)
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
            var dirPath = Path.Combine(Environment.CurrentDirectory, "images");
            var filePath = Path.Combine(dirPath, fileName);
            Directory.CreateDirectory(dirPath);

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