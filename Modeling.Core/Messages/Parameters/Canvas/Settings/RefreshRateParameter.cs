using Modeling.Core.Abstractions;
using System;

namespace Modeling.Core.Messages.Parameters.Canvas.Settings
{
    public sealed class RefreshRateParameter(TimeSpan refreshRate) : IDefaultCheck
    {
        public TimeSpan RefreshRate { get; init; } = refreshRate;

        public bool IsDefault() => false;
    }
}
