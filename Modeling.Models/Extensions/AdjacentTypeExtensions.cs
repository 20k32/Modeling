using Modeling.Models.Enums;

namespace Modeling.Models.Extensions
{
    public static class AdjacentTypeExtensions
    {
        public static AdjacentType Invert(this AdjacentType type) => type switch
        {
            AdjacentType.Inner => AdjacentType.Outer,
            AdjacentType.Outer => AdjacentType.Inner,
            AdjacentType.Nearby => AdjacentType.Nearby,
            _ => AdjacentType.None
        };
    }
}
