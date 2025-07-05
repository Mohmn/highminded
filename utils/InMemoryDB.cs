using System;
using System.ClientModel;
using highminded.models;
using OpenAI;
using OpenAI.Chat;

namespace highminded.utils;

public class InMemoryDb
{
    public static readonly InMemoryDb Obj = new InMemoryDb();

    internal readonly SettingsManager<SettingsViewModel> SettingsManager;
    internal readonly MainViewModel MainViewModel;
    internal readonly ChatViewModel ChatViewModel;
    internal readonly SettingsViewModel SettingsViewModel;
    internal ChatClient? ChatClient;

    // Initialize Singleton Class
    private InMemoryDb()
    {
        SettingsManager = new SettingsManager<SettingsViewModel>();

        MainViewModel = new MainViewModel();
        ChatViewModel = new ChatViewModel();
        SettingsViewModel = new SettingsViewModel();
        SettingsViewModel = SettingsManager.Settings;

        if (SettingsManager.Settings.ApiKey != string.Empty)
        {
            InitOpenAiClient();
        }
    }

    public void SaveSettings()
    {
        SettingsManager.Save();
        InitOpenAiClient();
    }

    private void InitOpenAiClient()
    {
        ChatClient = new ChatClient(
            model: SettingsManager.Settings.Model,
            credential: new ApiKeyCredential(SettingsManager.Settings.ApiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(SettingsManager.Settings.ApiUrl)
            }
        );
    }
}