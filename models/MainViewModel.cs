using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace highminded.models;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _appName = "High Minded";

    [ObservableProperty]
    private string _hideShortcutKey = OperatingSystem.IsMacOS() ? "OPTION + SHIFT + \\" : "ALT + SHIFT + \\";

    [ObservableProperty] private string _screenshotShortcutKey =
        OperatingSystem.IsMacOS() ? "OPTION + SHIFT + S" : "ALT + SHIFT + S";

    [ObservableProperty]
    private string _audioShortcutKey = OperatingSystem.IsMacOS() ? "OPTION + SHIFT + A" : "ALT + SHIFT + A";

    [ObservableProperty] private bool _isRecording = false;
    [ObservableProperty] private bool _isVisibleOnScreenshare = false;
}