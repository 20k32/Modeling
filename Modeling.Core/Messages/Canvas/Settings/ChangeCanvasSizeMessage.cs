using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using Windows.Foundation;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class ChangeCanvasSizeMessage(object sender, CanvasSizeParameter value) : ParametrizedMessage<CanvasSizeParameter>(sender, value)
    { }
}
