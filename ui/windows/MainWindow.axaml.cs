using Avalonia.Controls;
using System;
using System.Runtime.InteropServices;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using highminded.ui.controls;
using SharpHook;
using SharpHook.Data;

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
    private readonly TaskPoolGlobalHook _hook = new TaskPoolGlobalHook();

    public MainWindow()
    {
        InitializeComponent();
        UControl.Content = _chatUserControl;
        ChatBtnActive();
        HideOverlay();
        // Global Hotkey
        _hook.KeyPressed += OnKeyPressed;
        _hook.RunAsync();
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

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        bool hasCtrl = (e.RawEvent.Mask & EventMask.Ctrl) != EventMask.None;
        bool hasAlt = (e.RawEvent.Mask & EventMask.Alt) != EventMask.None;
        bool hasShift = (e.RawEvent.Mask & EventMask.Shift) != EventMask.None;
        bool hasH = e.Data.KeyCode == KeyCode.VcH;
        bool hasBackslash = e.Data.KeyCode == KeyCode.VcBackslash;
        bool hasS = e.Data.KeyCode == KeyCode.VcS;

        if (hasCtrl && hasShift && hasAlt && hasH)
        {
            ShowOverlay();
        }

        if (hasShift && hasS)
        {
            Dispatcher.UIThread.Post(() => { _chatUserControl.SendScreenshot(); });
        }

        if (hasShift && hasBackslash)
        {
            Dispatcher.UIThread.Post(() =>
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
            });
        }
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
    }

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
    }

    private void HideBtnClick(object? sender, RoutedEventArgs e)
    {
        Hide();
    }
}