using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
