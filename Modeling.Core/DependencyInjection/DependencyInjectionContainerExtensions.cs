using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.Dispatching;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Logging;
using Modeling.Core.Logging.Formatting;
using Modeling.Core.Serializer;
using Serilog;

namespace Modeling.Core.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            LoggerInitializer.Initialize();

            return services
                .AddSingleton<IDrawingSettingsProvider, DrawingSettingsProvider>()
                .AddSingleton<ISerializer, NewtonSoftSerializer>()
                .AddSingleton<IUserInterfaceThreadContext, UserInterfaceThreadContext>()
                .AddSingleton<ILogFormatter, LogFormatter>()
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));
        }
    }
}
