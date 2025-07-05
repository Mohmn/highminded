using CommunityToolkit.Mvvm.ComponentModel;

namespace highminded.models;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _appName = "High Minded";
    [ObservableProperty] private string _hideShortcutKey = "SHIFT + \\";
    [ObservableProperty] private string _screenshotShortcutKey = "SHIFT + S";
    [ObservableProperty] private string _audioShortcutKey = "SHIFT + A";
    [ObservableProperty] private bool _isRecording = false;
    [ObservableProperty] private bool _isHidden = true;
}