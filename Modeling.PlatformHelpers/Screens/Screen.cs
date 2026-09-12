using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace Modeling.PlatformHelpers.Screens
{
    public sealed class Screen
    {
        public nint Handle { get; }
        public bool IsPrimary { get; }
        public RectInt32 WorkingArea { get; }
        public RectInt32 Bounds { get; }
        public string DeviceName { get; }
        public double ScaleFactor { get; }

        Screen(nint handle)
        {
            var handleMonitor = new HMONITOR(handle);
            Handle = handle;

            var monitorInfoExtended = new MONITORINFOEXW();

            monitorInfoExtended.monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfoExtended);

            if (!GetMonitorInfo(handleMonitor, ref monitorInfoExtended.monitorInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            DeviceName = monitorInfoExtended.szDevice.ToString();

            Bounds = new RectInt32(monitorInfoExtended.monitorInfo.rcMonitor.left, monitorInfoExtended.monitorInfo.rcMonitor.top,
                monitorInfoExtended.monitorInfo.rcMonitor.right - monitorInfoExtended.monitorInfo.rcMonitor.left, monitorInfoExtended.monitorInfo.rcMonitor.bottom - monitorInfoExtended.monitorInfo.rcMonitor.top);

            WorkingArea = new RectInt32(monitorInfoExtended.monitorInfo.rcWork.left, monitorInfoExtended.monitorInfo.rcWork.top,
                monitorInfoExtended.monitorInfo.rcWork.right - monitorInfoExtended.monitorInfo.rcWork.left, monitorInfoExtended.monitorInfo.rcWork.bottom - monitorInfoExtended.monitorInfo.rcWork.top);

            IsPrimary = (monitorInfoExtended.monitorInfo.dwFlags & MONITORINFOF_PRIMARY) != 0; ;
        }

        public static unsafe IEnumerable<Screen> All
        {
            get
            {
                var all = new List<Screen>();

                EnumDisplayMonitors(
                    default(HDC),
                    (RECT?)null,
                    (m, _, _, _) =>
                    {
                        all.Add(new Screen(m));
                        return true;
                    },
                    default(LPARAM));

                return all;
            }
        }

        public override string ToString() => DeviceName;
        public static nint GetNearestFromWindow(nint hwnd) => MonitorFromWindow(new HWND(hwnd), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        public static nint GetDesktopMonitorHandle() => GetNearestFromWindow(GetDesktopWindow());
        public static nint GetShellMonitorHandle() => GetNearestFromWindow(GetShellWindow());
        public static Screen FromWindow(nint hwnd)
        {
            var h = MonitorFromWindow(new HWND(hwnd), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
            return h != nint.Zero ? new Screen(h) : null;
        }
    }
}
