using CommunityToolkit.Mvvm.ComponentModel;

namespace highminded.models;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty] private string _content = "";
    [ObservableProperty] private string _prompt = "";
    [ObservableProperty] private string _promptWatermark = "Ask me anything...";
}