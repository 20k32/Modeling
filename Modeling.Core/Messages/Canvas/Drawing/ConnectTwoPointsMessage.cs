using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas;
using System.Collections.Generic;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class ConnectTwoPointsMessage : ParametrizedMessage<TwoPointsMessageParameter>
    {
        public ConnectTwoPointsMessage(object sender, TwoPointsMessageParameter value) : base(sender, value)
        { }

        public override bool IsDefault()
        {
            return base.IsDefault();
        }
    }
}
