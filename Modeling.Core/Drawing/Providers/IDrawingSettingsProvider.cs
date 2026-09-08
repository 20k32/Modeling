using Modeling.Core.Abstractions;
using Modeling.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Drawing.Providers
{
    public interface IDrawingSettingsProvider : IAsyncInitializer
    {
        IDrawingSettings Settings { get; }

        Task SaveSettingsAsync();
        Task LoadSettingsAsync();
    }
}
