using System;
using System.IO;
using Path = Avalonia.Controls.Shapes.Path;

namespace highminded.utils;

public class InMemoryDb
{
    // Initialize Singleton Class
    InMemoryDb() { }

    public static readonly InMemoryDb Obj = new InMemoryDb();

    public SettingsManager<AppSettings> settingsManager = new SettingsManager<AppSettings>();
}