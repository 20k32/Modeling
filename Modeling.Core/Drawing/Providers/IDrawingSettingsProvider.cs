using Modeling.Core.Abstractions;
using Modeling.Core.Settings;
using System.Threading.Tasks;

namespace Modeling.Core.Drawing.Providers
{
    public interface IDrawingSettingsProvider : IAsyncInitializer, ISettingsChanged
    {
        IDrawingSettings Settings { get; }

        Task SaveSettingsAsync();
        Task LoadSettingsAsync();
        void SetFileToken(string token);
    }
}
