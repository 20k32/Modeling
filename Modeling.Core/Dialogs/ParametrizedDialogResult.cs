namespace Modeling.Core.Dialogs
{
    public sealed class ParametrizedDialogResult<T>(DialogAction action, T value) : DialogResult(action)
    {
        public readonly T Value = value;
    }
}
