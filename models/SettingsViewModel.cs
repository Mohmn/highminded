using CommunityToolkit.Mvvm.ComponentModel;

namespace highminded.models;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private string _model = "";
    [ObservableProperty] private string _apiUrl = "";
    [ObservableProperty] private string _apiKey = "";
    [ObservableProperty] private string _screenshotPrompt = "";
    [ObservableProperty] private string _audioPrompt = "";
}