using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.DependencyInjection;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.DrawingMessageInterpreter;
using Modeling.Models.Drawing.DrawingPipeline;
using Modeling.Models.Drawing.Figures;
using Modeling.PlatformHelpers.DependencyInjection;

namespace Modeling.Models.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterModelsServices(this IServiceCollection services)
            => services.AddTransient<IPointGeometry, PointGeometry>()
            .AddTransient<IFigure, Figure>()
            .AddTransient<IDrawingPipeline, DrawingPipeline>()
            .AddSingleton<IDrawingMessageInterpreter, DrawingMessageInterpreter>()
            .RegisterCoreServices()
            .RegisterPlatformHelpers();
    }
}
