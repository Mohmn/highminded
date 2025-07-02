using System;
using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace highminded.utils;

public class InMemoryDb
{
    internal ChatClient ChatClient;
    internal SettingsManager<AppSettings> SettingsManager;

    // Initialize Singleton Class
    private InMemoryDb()
    {
        SettingsManager = new SettingsManager<AppSettings>();
        if (SettingsManager.Settings.ApiKey != null)
        {
            InitOpenAIClient();
        }
    }

    public void InitOpenAIClient()
    {
        ChatClient = new ChatClient(
            model: SettingsManager.Settings.Model,
            credential: new ApiKeyCredential(SettingsManager.Settings.ApiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(SettingsManager.Settings.ApiURL)
            }
        );
    }

    public void SaveSettings(AppSettings settings)
    {
        SettingsManager.Settings = settings;
        SettingsManager.Save();
        InitOpenAIClient();
    }

    public static readonly InMemoryDb Obj = new InMemoryDb();
}