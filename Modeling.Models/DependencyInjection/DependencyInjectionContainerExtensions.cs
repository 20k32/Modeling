using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.Abstractions;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.DependencyInjection;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Collections.Drawings;
using Modeling.Models.Drawing.DrawingMessageInterpreter;
using Modeling.Models.Drawing.DrawingPipeline;
using Modeling.Models.Drawing.Figures;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Navigation;
using Modeling.Models.UserInterface;
using Modeling.PlatformHelpers.DependencyInjection;

namespace Modeling.Models.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterModelsServices(this IServiceCollection services)
            => services.AddTransient<IPointGeometryCollection, PointGeometryCollection>()
            .AddTransient<CirclePointGeometry, CirclePointGeometry>()
            .AddTransient<LinePointGeometry, LinePointGeometry>()
            .AddTransient<IPointGeometryCreationFactory, PointGeometryCreationFactory>()
            .AddTransient<IFigure, Figure>()
            .AddTransient<IDrawingPipeline, DrawingPipeline>()
            .AddSingleton<IDrawingMessageInterpreter, DrawingMessageInterpreter>()
            .AddSingleton<IUserInterfaceConstantsProvider, UserInterfaceConstantsProvider>()
            .AddSingleton<INavigationProvider, NavigationProvider>()
            .RegisterCoreServices()
            .RegisterPlatformHelpers();
    }
}
