using System;
using System.Collections.Generic;
using SharpHook;
using SharpHook.Data;

namespace highminded.utils;

public enum ModifierKey
{
    Control,
    Shift,
    Alt,
    Meta
}

public class PreciseKeyBindings
{
    private readonly IGlobalHook _hook;
    private readonly Dictionary<KeyBinding, Action> _keyBindings;
    private readonly HashSet<KeyCode> _pressedKeys;

    public PreciseKeyBindings()
    {
        _hook = new TaskPoolGlobalHook();
        _keyBindings = new Dictionary<KeyBinding, Action>();
        _pressedKeys = new HashSet<KeyCode>();

        // Subscribe to key events
        _hook.KeyPressed += OnKeyPressed;
        _hook.KeyReleased += OnKeyReleased;
    }

    public void AddKeyBinding(KeyBinding binding, Action action)
    {
        _keyBindings[binding] = action;
    }

    public void Start()
    {
        _hook.RunAsync();
    }

    public void Stop()
    {
        _hook.Dispose();
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        _pressedKeys.Add(e.Data.KeyCode);

        // Check if any key binding matches the current pressed keys
        foreach (var kvp in _keyBindings)
        {
            if (IsExactMatch(kvp.Key))
            {
                kvp.Value.Invoke();
                break; // Only trigger the first match
            }
        }
    }

    private void OnKeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        _pressedKeys.Remove(e.Data.KeyCode);
    }

    private bool IsExactMatch(KeyBinding binding)
    {
        // Get currently pressed modifiers
        var pressedModifiers = GetPressedModifiers();

        // Check if the main key is pressed
        if (!_pressedKeys.Contains(binding.Key))
            return false;

        // Check if modifiers match exactly
        if (pressedModifiers.Count != binding.Modifiers.Count)
            return false;

        foreach (var modifier in binding.Modifiers)
        {
            if (!pressedModifiers.Contains(modifier))
                return false;
        }

        return true;
    }

    private HashSet<ModifierKey> GetPressedModifiers()
    {
        var modifiers = new HashSet<ModifierKey>();

        // Check for Control
        if (_pressedKeys.Contains(KeyCode.VcLeftControl) || _pressedKeys.Contains(KeyCode.VcRightControl))
            modifiers.Add(ModifierKey.Control);

        // Check for Shift
        if (_pressedKeys.Contains(KeyCode.VcLeftShift) || _pressedKeys.Contains(KeyCode.VcRightShift))
            modifiers.Add(ModifierKey.Shift);

        // Check for Alt
        if (_pressedKeys.Contains(KeyCode.VcLeftAlt) || _pressedKeys.Contains(KeyCode.VcRightAlt))
            modifiers.Add(ModifierKey.Alt);

        // Check for Meta/Windows key
        if (_pressedKeys.Contains(KeyCode.VcLeftMeta) || _pressedKeys.Contains(KeyCode.VcRightMeta))
            modifiers.Add(ModifierKey.Meta);

        return modifiers;
    }
}

public class KeyBinding : IEquatable<KeyBinding>
{
    public KeyCode Key { get; }
    public HashSet<ModifierKey> Modifiers { get; }

    public KeyBinding(KeyCode key, params ModifierKey[] modifiers)
    {
        Key = key;
        Modifiers = new HashSet<ModifierKey>(modifiers);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as KeyBinding);
    }

    public bool Equals(KeyBinding? other)
    {
        if (other == null) return false;
        if (Key != other.Key) return false;
        if (Modifiers.Count != other.Modifiers.Count) return false;

        foreach (var modifier in Modifiers)
        {
            if (!other.Modifiers.Contains(modifier))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = Key.GetHashCode();
        foreach (var modifier in Modifiers)
        {
            hash ^= modifier.GetHashCode();
        }

        return hash;
    }

    public override string ToString()
    {
        var modStr = string.Join(" + ", Modifiers);
        return string.IsNullOrEmpty(modStr) ? Key.ToString() : $"{modStr} + {Key}";
    }
}