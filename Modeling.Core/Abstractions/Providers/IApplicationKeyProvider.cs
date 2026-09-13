namespace Modeling.Core.Abstractions.Providers
{
    public interface IApplicationKeyProvider : IAsyncInitializer
    {
        string DrawingSettingsTokenKey { get; }
    }
}
