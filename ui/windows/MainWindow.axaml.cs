using Avalonia.Controls;
using System;
using System.Runtime.InteropServices;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using highminded.ui.controls;
using highminded.utils;
using SharpHook.Data;
using KeyBinding = highminded.utils.KeyBinding;

namespace highminded.ui.windows;

public partial class MainWindow : Window
{
    // MacOS
    private const string ObjCRuntime = "/usr/lib/libobjc.dylib";

    [DllImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
    private static extern void ObjcMsgSendInt(nint receiver, nint selector, int value);

    [DllImport(ObjCRuntime, EntryPoint = "sel_registerName")]
    private static extern nint RegisterName(string name);

    // Windows
    [DllImport("user32.dll")]
    private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

    // Controls
    private readonly ChatUserControl _chatUserControl = new ChatUserControl();
    private readonly SettingsUserControl _settingsUserControl = new SettingsUserControl();

    // Hotkey
    private readonly PreciseKeyBindings _keyBindings = new PreciseKeyBindings();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = InMemoryDb.Obj.MainViewModel;

        UControl.Content = _chatUserControl;
        ChatBtnActive();
        HideOverlay();
        RegisterKeyBindings();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void ChatBtnClick(object? sender, RoutedEventArgs e)
    {
        UControl.Content = _chatUserControl;
        ChatBtnActive();
    }

    private void SettingBtnClick(object? sender, RoutedEventArgs e)
    {
        UControl.Content = _settingsUserControl;
        SettingsBtnActive();
    }

    private void ChatBtnActive()
    {
        ChatBtn.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
        SettingsBtn.Background = new SolidColorBrush(Colors.Transparent);
    }

    private void SettingsBtnActive()
    {
        ChatBtn.Background = new SolidColorBrush(Colors.Transparent);
        SettingsBtn.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
    }

    private void ShowOverlay()
    {
        var handle = TryGetPlatformHandle();

        if (handle is null || handle.Handle == IntPtr.Zero)
        {
            Console.WriteLine("Invalid window handle.");
            return;
        }

        var hwnd = handle.Handle;

        switch (handle.HandleDescriptor)
        {
            case "HWND":
            {
                const uint include = 0x00000000;
                SetWindowDisplayAffinity(hwnd, include);
                break;
            }
            case "NSWindow":
            {
                const int include = 2;
                var sharingTypeSelector = RegisterName("setSharingType:");
                ObjcMsgSendInt(hwnd, sharingTypeSelector, include);
                break;
            }
        }

        InMemoryDb.Obj.MainViewModel.IsVisibleOnScreenshare = true;
    }

    // Here the sauce lol, Cheers to our curiosity!
    private void HideOverlay()
    {
        var handle = TryGetPlatformHandle();

        if (handle is null || handle.Handle == IntPtr.Zero)
        {
            Console.WriteLine("Invalid window handle.");
            return;
        }

        var hwnd = handle.Handle;

        switch (handle.HandleDescriptor)
        {
            case "HWND":
            {
                const uint exclude = 0x00000011;
                SetWindowDisplayAffinity(hwnd, exclude);
                break;
            }
            case "NSWindow":
            {
                const int exclude = 0;
                var sharingTypeSelector = RegisterName("setSharingType:");
                ObjcMsgSendInt(hwnd, sharingTypeSelector, exclude);
                break;
            }
        }

        InMemoryDb.Obj.MainViewModel.IsVisibleOnScreenshare = false;
    }

    private void HideBtnClick(object? sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void RegisterKeyBindings()
    {
        _keyBindings.AddKeyBinding(
            new KeyBinding(KeyCode.VcH, ModifierKey.Control, ModifierKey.Alt, ModifierKey.Shift),
            () => Dispatcher.UIThread.Post(() =>
            {
                if (InMemoryDb.Obj.MainViewModel.IsVisibleOnScreenshare)
                {
                    HideOverlay();
                }
                else
                {
                    ShowOverlay();
                }
            })
        );

        _keyBindings.AddKeyBinding(
            new KeyBinding(KeyCode.VcQ, ModifierKey.Alt, ModifierKey.Shift),
            () => Dispatcher.UIThread.Post(() => Environment.Exit(0))
        );

        _keyBindings.AddKeyBinding(
            new KeyBinding(KeyCode.VcBackslash, ModifierKey.Alt, ModifierKey.Shift),
            () => Dispatcher.UIThread.Post(() =>
            {
                if (WindowState == WindowState.Minimized || !IsVisible)
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                    Topmost = true;
                }
                else
                {
                    Hide();
                }
            })
        );

        _keyBindings.AddKeyBinding(
            new KeyBinding(KeyCode.VcA, ModifierKey.Alt, ModifierKey.Shift),
            () => Dispatcher.UIThread.Post(() =>
            {
                if (!InMemoryDb.Obj.MainViewModel.IsRecording)
                {
                    _chatUserControl.StartRecord();
                }
                else
                {
                    _chatUserControl.StopRecord();
                }
            })
        );

        _keyBindings.AddKeyBinding(
            new KeyBinding(KeyCode.VcS, ModifierKey.Alt, ModifierKey.Shift),
            () => Dispatcher.UIThread.Post(() => _chatUserControl.SendScreenshot())
        );

        _keyBindings.Start();
    }
}