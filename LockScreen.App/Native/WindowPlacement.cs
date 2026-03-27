using System.Runtime.InteropServices;

namespace LockScreen.App.Native;

internal static class WindowPlacement
{
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpShowWindow = 0x0040;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint uFlags);

    public static void MoveToScreenBounds(IntPtr windowHandle, System.Drawing.Rectangle bounds)
    {
        SetWindowPos(
            windowHandle,
            IntPtr.Zero,
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            SwpNoZOrder | SwpNoActivate | SwpShowWindow);
    }
}
