using System.Drawing.Drawing2D;

namespace TaskBarHideSwitch;

/// <summary>
/// Application context that manages the system tray icon and menu.
/// </summary>
public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _startupMenuItem;

    public TrayApplicationContext()
    {
        // Create context menu
        _startupMenuItem = new ToolStripMenuItem("开机启动")
        {
            CheckOnClick = true,
            Checked = StartupHelper.IsStartupEnabled()
        };
        _startupMenuItem.Click += OnStartupMenuItemClick;

        var exitMenuItem = new ToolStripMenuItem("退出");
        exitMenuItem.Click += OnExitMenuItemClick;

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(_startupMenuItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(exitMenuItem);

        // Create notify icon
        _notifyIcon = new NotifyIcon
        {
            Icon = CreateTrayIcon(),
            ContextMenuStrip = contextMenu,
            Visible = true
        };

        UpdateTooltip();

        _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;
    }

    private void OnNotifyIconDoubleClick(object? sender, EventArgs e)
    {
        TaskbarHelper.ToggleAutoHide();
        UpdateTooltip();
    }

    private void OnStartupMenuItemClick(object? sender, EventArgs e)
    {
        StartupHelper.SetStartup(_startupMenuItem.Checked);
    }

    private void OnExitMenuItemClick(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        Application.Exit();
    }

    private void UpdateTooltip()
    {
        var isAutoHide = TaskbarHelper.GetAutoHideEnabled();
        _notifyIcon.Text = isAutoHide ? "任务栏: 自动隐藏" : "任务栏: 始终显示";
    }

    /// <summary>
    /// Creates a simple tray icon programmatically.
    /// Icon design: A taskbar-like rectangle with an arrow indicating hide/show.
    /// </summary>
    private static Icon CreateTrayIcon()
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        // Draw taskbar representation (bottom rectangle)
        var taskbarRect = new Rectangle(2, size - 10, size - 4, 8);
        using var taskbarBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        graphics.FillRectangle(taskbarBrush, taskbarRect);

        // Draw border
        using var borderPen = new Pen(Color.FromArgb(100, 100, 100), 1);
        graphics.DrawRectangle(borderPen, taskbarRect);

        // Draw up/down arrow (indicating toggle)
        using var arrowPen = new Pen(Color.White, 2);
        var arrowCenterX = size / 2;
        var arrowTop = 4;
        var arrowBottom = size - 14;

        // Arrow line
        graphics.DrawLine(arrowPen, arrowCenterX, arrowTop, arrowCenterX, arrowBottom);

        // Arrow head (pointing up)
        graphics.DrawLine(arrowPen, arrowCenterX - 4, arrowTop + 4, arrowCenterX, arrowTop);
        graphics.DrawLine(arrowPen, arrowCenterX + 4, arrowTop + 4, arrowCenterX, arrowTop);

        // Arrow head (pointing down)
        graphics.DrawLine(arrowPen, arrowCenterX - 4, arrowBottom - 4, arrowCenterX, arrowBottom);
        graphics.DrawLine(arrowPen, arrowCenterX + 4, arrowBottom - 4, arrowCenterX, arrowBottom);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
