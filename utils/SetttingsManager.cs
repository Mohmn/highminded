using System;
using System.IO;
using System.Text.Json;

namespace highminded.utils;

public class SettingsManager<T> where T : class, new()
{
    private readonly string _settingsPath;
    public T Settings { get; internal set; }

    public SettingsManager()
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