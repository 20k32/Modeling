using System;

namespace Modeling.Models.Enums
{
    [Flags]
    public enum DrawingMessageType : ulong
    {
        NoAction = 0,
        ClearCanvas = 1ul << 0,
        DrawPolygon = 1ul << 1,
    }
}
