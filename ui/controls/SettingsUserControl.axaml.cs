using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using highminded.utils;

namespace highminded.ui.controls;

public partial class SettingsUserControl : UserControl
{
    public SettingsUserControl()
    {
        InitializeComponent();

        ModelTextBox.Text = InMemoryDb.Obj.settingsManager.Settings.Model;
        ApiUrlTextBox.Text = InMemoryDb.Obj.settingsManager.Settings.ApiURL;
        ApiKeyTextBox.Text = InMemoryDb.Obj.settingsManager.Settings.ApiKey;
    }

    private void SaveSettingsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        InMemoryDb.Obj.settingsManager.Settings.Model = ModelTextBox.Text;
        InMemoryDb.Obj.settingsManager.Settings.ApiURL= ApiUrlTextBox.Text;
        InMemoryDb.Obj.settingsManager.Settings.ApiKey = ApiKeyTextBox.Text;
       InMemoryDb.Obj.settingsManager.Save(); 
    }
}