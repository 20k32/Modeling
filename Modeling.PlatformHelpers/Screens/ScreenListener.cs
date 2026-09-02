namespace Modeling.PlatformHelpers.Monitor
{
    internal class ScreenListener
    {
        /*public ReadOnlyCollection<Screen> Monitors => new(Screen.All.ToArray());

        public List<MonitorLocation> MonitorLocations
        {
            get
            {
                var monitorsCopy = Monitors.AsReadOnly();

                var tempList = new List<MonitorLocation>(monitorsCopy.Count);

                foreach (var monitor in monitorsCopy)
                {
                    var monitorLocation = new MonitorLocation(monitor.Bounds, monitor.IsPrimary, monitor.DeviceName, monitor.Handle);
                    tempList.Add(monitorLocation);
                }

                return tempList;
            }
        }*/
    }
}
