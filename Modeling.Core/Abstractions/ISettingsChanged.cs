namespace Modeling.Core.Abstractions
{
    public interface ISettingsChanged
    {
        event ActionEventHandler SettingsChanged;

        bool ShouldInvokeSettingsChanged { get; set; }
    }
}
