using Avalonia.Controls;
using System;
using System.ClientModel;
using System.Text;
using Avalonia.Input;
using highminded.utils;
using OpenAI.Chat;
using OpenAI;
using Markdig;
using Markdown.ColorCode;

namespace highminded.ui.controls;

public partial class ChatUserControl : UserControl
{
    
    // OpenAI
    private readonly OpenAI.Chat.ChatClient _client = null!;
    private readonly MarkdownPipeline _pipeline = null!; 
    
    public ChatUserControl()
    {
        InitializeComponent();
        
        _client = new ChatClient(
            model: InMemoryDb.Obj.settingsManager.Settings.Model,
            credential: new ApiKeyCredential(InMemoryDb.Obj.settingsManager.Settings.ApiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(InMemoryDb.Obj.settingsManager.Settings.ApiURL)
            });

        _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode().Build(); 
    }
    
    private async void PromptBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key != Key.Enter) return;

            var prompt = PromptBox.Text;
            if (prompt is null) return;
            PromptBox.Clear();

            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates =
                _client.CompleteChatStreamingAsync(prompt);

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
        catch (Exception err)
        {
            ResultBlock.Text = err.Message;
        }
    }
}