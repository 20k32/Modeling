namespace Modeling.Core.Dispatching.Messages
{
    public sealed class MainThreadMessageState<T>
    {
        public static readonly MainThreadMessageState<T> Default = new(default);

        public readonly T Value;

        public MainThreadMessageState(T value = default) => Value = value;
    }
}
