using CommunityToolkit.Mvvm.ComponentModel;

namespace highminded.models;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _appName = "High Minded";
    [ObservableProperty] private string _hideShortcutKey = "ALT + SHIFT + \\";
    [ObservableProperty] private string _screenshotShortcutKey = "ALT + SHIFT + S";
    [ObservableProperty] private string _audioShortcutKey = "ALT + SHIFT + A";
    [ObservableProperty] private bool _isRecording = false;
    [ObservableProperty] private bool _isHidden = true;
}