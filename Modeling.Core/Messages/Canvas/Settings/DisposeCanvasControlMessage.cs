using Modeling.Core.Messages.Base.SynchronousMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class DisposeCanvasControlMessage : Message
    {
        public DisposeCanvasControlMessage(object sender) : base(sender)
        { }
    }
}
