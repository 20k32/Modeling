using Modeling.Core.Messages.Base.SynchronousMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class InitializeCanvasControlMessage : Message
    {
        public InitializeCanvasControlMessage(object sender) : base(sender)
        { }
    }
}
