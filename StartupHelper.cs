using Microsoft.Win32;

namespace TaskBarHideSwitch;

/// <summary>
/// Helper class for managing application startup with Windows using registry.
/// </summary>
public static class StartupHelper
{
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "TaskBarHideSwitch";

    /// <summary>
    /// Gets whether the application is configured to start with Windows.
    /// </summary>
    /// <returns>True if startup is enabled, false otherwise.</returns>
    public static bool IsStartupEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
        if (key == null) return false;

        var value = key.GetValue(AppName) as string;
        if (string.IsNullOrEmpty(value)) return false;

        // Verify the path matches current executable
        var currentPath = GetExecutablePath();
        return string.Equals(value, currentPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Sets whether the application should start with Windows.
    /// </summary>
    /// <param name="enabled">True to enable startup, false to disable.</param>
    public static void SetStartup(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
        if (key == null) return;

        if (enabled)
        {
            var executablePath = GetExecutablePath();
            key.SetValue(AppName, executablePath);
        }
        else
        {
            key.DeleteValue(AppName, false);
        }
    }

    /// <summary>
    /// Toggles the startup state.
    /// </summary>
    /// <returns>The new startup state after toggling.</returns>
    public static bool ToggleStartup()
    {
        var currentState = IsStartupEnabled();
        SetStartup(!currentState);
        return !currentState;
    }

    private static string GetExecutablePath()
    {
        return Environment.ProcessPath ?? Application.ExecutablePath;
    }
}
