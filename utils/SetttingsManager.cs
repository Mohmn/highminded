using System;
using System.IO;
using System.Text.Json;

namespace highminded.utils;

public class AppSettings
{
    public string Model { get; set; }
    public string ApiURL { get; set; }
    public string ApiKey { get; set; }
    public string ScreenshotPrompt { get; set; }
    
}

public class SettingsManager<T> where T : class, new()
{
    private readonly string _settingsPath;
    public T Settings { get; internal set; }

    public SettingsManager(string appName = "highminded")
    {
       _settingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");
       Settings = Load();
    } 
    
    private T Load()
    {
        if (!File.Exists(_settingsPath))
            return new T();

        try
        {
            string json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch
        {
            return new T(); // Fallback to default settings if error occurs
        }
    }

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(Settings, options);
        File.WriteAllText(_settingsPath, json);
    }
}