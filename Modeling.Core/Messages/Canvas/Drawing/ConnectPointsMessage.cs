using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class ConnectPointsMessage : ParametrizedMessage<PointListMessageParameter>
    {
        public ConnectPointsMessage(object sender, PointListMessageParameter value) : base(sender, value)
        { }
    }
}
