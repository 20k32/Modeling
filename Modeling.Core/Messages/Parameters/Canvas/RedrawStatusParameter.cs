using Modeling.Core.Abstractions;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class RedrawStatusParameter(bool paused) : IDefaultCheck
    {
        public bool Paused { get; init; } = paused;

        public bool IsDefault() => false;
    }
}
