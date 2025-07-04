using Avalonia.Controls;
using Avalonia.Interactivity;
using highminded.utils;

namespace highminded.ui.controls;

public partial class SettingsUserControl : UserControl
{
    public SettingsUserControl()
    {
        InitializeComponent();

        ModelTextBox.Text = InMemoryDb.Obj.SettingsManager.Settings.Model;
        ApiUrlTextBox.Text = InMemoryDb.Obj.SettingsManager.Settings.ApiURL;
        ApiKeyTextBox.Text = InMemoryDb.Obj.SettingsManager.Settings.ApiKey;
        ScreenshotPromptTextBox.Text = InMemoryDb.Obj.SettingsManager.Settings.ScreenshotPrompt;
        AudioPromptTextbox.Text = InMemoryDb.Obj.SettingsManager.Settings.AudioPrompt;
    }

    private void SaveSettingsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        InMemoryDb.Obj.SaveSettings(new AppSettings()
        {
            ApiKey = ApiKeyTextBox.Text, ApiURL = ApiUrlTextBox.Text, Model = ModelTextBox.Text,
            ScreenshotPrompt = ScreenshotPromptTextBox.Text, AudioPrompt = AudioPromptTextbox.Text
        });
    }
}