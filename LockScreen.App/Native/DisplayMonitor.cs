using System.Runtime.InteropServices;

namespace LockScreen.App.Native;

public readonly record struct ScreenBounds(int Left, int Top, int Width, int Height);

internal readonly record struct DisplayMonitor(ScreenBounds Bounds)
{
    public static IReadOnlyList<DisplayMonitor> GetAll()
    {
        var monitors = new List<DisplayMonitor>();
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (monitorHandle, _, _, _) =>
        {
            var info = new MonitorInfoEx();
            info.Size = Marshal.SizeOf<MonitorInfoEx>();
            if (GetMonitorInfo(monitorHandle, ref info))
            {
                var bounds = new ScreenBounds(
                    info.Monitor.Left,
                    info.Monitor.Top,
                    info.Monitor.Right - info.Monitor.Left,
                    info.Monitor.Bottom - info.Monitor.Top);
                monitors.Add(new DisplayMonitor(bounds));
            }

            return true;
        }, IntPtr.Zero);

        return monitors;
    }

    private delegate bool MonitorEnumProc(
        IntPtr monitorHandle,
        IntPtr hdcMonitor,
        IntPtr lprcMonitor,
        IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(
        IntPtr hdc,
        IntPtr lprcClip,
        MonitorEnumProc callback,
        IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(
        IntPtr hMonitor,
        ref MonitorInfoEx monitorInfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfoEx
    {
        public int Size;
        public Rect Monitor;
        public Rect WorkArea;
        public uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
