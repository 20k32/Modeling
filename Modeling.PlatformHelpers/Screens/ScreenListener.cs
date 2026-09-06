using Modeling.Core.Windowing;
using Modeling.PlatformHelpers.Screens;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;

namespace Modeling.PlatformHelpers.Monitor
{
    internal class ScreenListener : IScreenListener
    {
        ReadOnlyCollection<Screen> _monitors => new([.. Screen.All]);

        public List<ScreenArea> Locations
        {
            get
            {
                var monitorsCopy = _monitors.AsReadOnly();

                var tempList = new List<ScreenArea>(monitorsCopy.Count);

                foreach (var monitor in monitorsCopy)
                {
                    var monitorLocation = new ScreenArea(monitor.Bounds,
                        monitor.IsPrimary,
                        monitor.DeviceName,
                        monitor.Handle,
                        size: new Size(monitor.Bounds.Width, monitor.Bounds.Height),
                        scaledSize: new Size(monitor.WorkingArea.Width, monitor.WorkingArea.Height));

                    tempList.Add(monitorLocation);
                }

                return tempList;
            }
        }
    }
}
