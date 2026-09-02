namespace Modeling.PlatformHelpers.Monitor
{
    public sealed class Screen
    {
        /*private Screen(nint handle)
        {
            Handle = handle;
            var monitorInfo = new MONITORINFOEXW();
            monitorInfo.monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            if (!GetMonitorInfo(handle, ref monitorInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            DeviceName = monitorInfo.szDevice.ToString();

            Bounds = new RectInt32(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top,
                monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top);

            WorkingArea = new RectInt32(monitorInfo.rcWork.left, monitorInfo.rcWork.top,
                monitorInfo.rcWork.right - monitorInfo.rcWork.left, monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);

            IsPrimary = monitorInfo.MONITORINFOF.dwFlags.HasFlag(MONITORINFOF.MONITORINFOF_PRIMARY);
        }

        public nint Handle { get; }
        public bool IsPrimary { get; }
        public RectInt32 WorkingArea { get; }
        public RectInt32 Bounds { get; }
        public string DeviceName { get; }
        public double ScaleFactor { get; }

        public static IEnumerable<Screen> All
        {
            get
            {
                var all = new List<Screen>();
                EnumDisplayMonitors(nint.Zero, nint.Zero, (m, h, rc, p) =>
                {
                    all.Add(new Screen(m));
                    return true;
                }, nint.Zero);
                return all;
            }
        }

        public override string ToString() => DeviceName;
        public static nint GetNearestFromWindow(nint hwnd) => MonitorFromWindow(hwnd, MFW.MONITOR_DEFAULTTONEAREST);
        public static nint GetDesktopMonitorHandle() => GetNearestFromWindow(GetDesktopWindow());
        public static nint GetShellMonitorHandle() => GetNearestFromWindow(GetShellWindow());
        public static Screen FromWindow(nint hwnd, MFW flags = MFW.MONITOR_DEFAULTTONULL)
        {
            nint h = MonitorFromWindow(hwnd, flags);
            return h != nint.Zero ? new Screen(h) : null;
        }*/
    }
}
