using Modeling.Core.Abstractions;
using Modeling.Core.Settings;
using System.Threading.Tasks;

namespace Modeling.Core.Drawing.Providers
{
    public interface IDrawingSettingsProvider : IAsyncInitializer
    {
        IDrawingSettings Settings { get; }
        event ActionEventHandler SettingsChanged;

        Task SaveSettingsAsync();
        Task LoadSettingsAsync();
        void SetFileToken(string token);
    }
}
