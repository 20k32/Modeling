using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Messages.ViewModels
{
    public sealed class ChangeCanvasSettingsMessage(object sender, UpdateDrawingsParameter value) : ParametrizedMessage<UpdateDrawingsParameter>(sender, value)
    { }
}
