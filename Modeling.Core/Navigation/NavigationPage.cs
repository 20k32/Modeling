using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Navigation
{
    [Flags]
    public enum NavigationPage : ulong
    {
        None = 0,
        ResizingPage = 1ul << 1,
        AffineTransformPage = 1ul << 2,
        EuclideanTransformPage = 1ul << 3,
        ProjectiveTransformPage = 1ul << 4,
        SettingsPage  = 1ul << 5,
    }
}
