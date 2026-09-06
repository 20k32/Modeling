using Modeling.Core.Windowing;
using System.Collections.Generic;

namespace Modeling.PlatformHelpers.Screens
{
    public interface IScreenListener
    {
        List<ScreenArea> Locations { get; }
    }
}
