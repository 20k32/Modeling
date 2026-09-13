using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;

namespace Modeling.Core.Windowing
{
    public sealed class ScreenArea
    {
        public readonly nint HandleMonitor;
        public readonly string DeviceName;
        public readonly RectInt32 Location;
        public readonly bool IsPrimary;
        public readonly float ScaleX;
        public readonly float ScaleY;
        public readonly Size Size;
        public readonly Size ScaledSize;
        public readonly float FramesPerSecond;

        public ScreenArea(RectInt32 location, bool isPrimary, string deviceName, nint handleMonitor, Size size, Size scaledSize)
        {
            Location = location;
            IsPrimary = isPrimary;
            DeviceName = deviceName ?? Guid.NewGuid().ToString();
            HandleMonitor = handleMonitor;
            Size = size;
            ScaledSize = scaledSize;
        }
    }
}
