using System.Runtime.InteropServices;

namespace TaskBarHideSwitch;

/// <summary>
/// Helper class for controlling Windows taskbar auto-hide functionality using Shell API.
/// </summary>
public static class TaskbarHelper
{
    #region Windows API Constants

    private const uint ABM_GETSTATE = 0x04;
    private const uint ABM_SETSTATE = 0x0A;
    private const int ABS_AUTOHIDE = 0x01;
    private const int ABS_ALWAYSONTOP = 0x02;

    #endregion

    #region Windows API Structures

    [StructLayout(LayoutKind.Sequential)]
    private struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public int lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    #endregion

    #region Windows API Imports

    [DllImport("shell32.dll", SetLastError = true)]
    private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

    #endregion

    /// <summary>
    /// Gets whether taskbar auto-hide is currently enabled.
    /// </summary>
    /// <returns>True if auto-hide is enabled, false otherwise.</returns>
    public static bool GetAutoHideEnabled()
    {
        var appBarData = CreateAppBarData();
        var state = SHAppBarMessage(ABM_GETSTATE, ref appBarData);
        return (state & ABS_AUTOHIDE) == ABS_AUTOHIDE;
    }

    /// <summary>
    /// Sets the taskbar auto-hide state.
    /// </summary>
    /// <param name="enabled">True to enable auto-hide, false to disable.</param>
    public static void SetAutoHide(bool enabled)
    {
        var appBarData = CreateAppBarData();

        // Get current state to preserve ABS_ALWAYSONTOP if set
        var currentState = (int)SHAppBarMessage(ABM_GETSTATE, ref appBarData);

        if (enabled)
        {
            appBarData.lParam = ABS_AUTOHIDE | (currentState & ABS_ALWAYSONTOP);
        }
        else
        {
            appBarData.lParam = currentState & ABS_ALWAYSONTOP;
        }

        SHAppBarMessage(ABM_SETSTATE, ref appBarData);
    }

    /// <summary>
    /// Toggles the taskbar auto-hide state.
    /// </summary>
    /// <returns>The new auto-hide state after toggling.</returns>
    public static bool ToggleAutoHide()
    {
        var currentState = GetAutoHideEnabled();
        SetAutoHide(!currentState);
        return !currentState;
    }

    private static APPBARDATA CreateAppBarData()
    {
        var appBarData = new APPBARDATA
        {
            cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)),
            hWnd = FindWindow("Shell_TrayWnd", null)
        };
        return appBarData;
    }
}
