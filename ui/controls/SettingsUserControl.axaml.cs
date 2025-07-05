using Avalonia.Controls;
using Avalonia.Interactivity;
using highminded.utils;

namespace highminded.ui.controls;

public partial class SettingsUserControl : UserControl
{
    public SettingsUserControl()
    {
        InitializeComponent();
        DataContext = InMemoryDb.Obj.SettingsViewModel;
    }

    private void SaveSettingsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        InMemoryDb.Obj.SaveSettings();
    }
}