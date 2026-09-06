using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.Dispatching;
using Modeling.Core.Logging;
using Modeling.Core.Logging.Formatting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            LoggerInitializer.Initialize();

            return services.AddSingleton<IUserInterfaceThreadContext, UserInterfaceThreadContext>()
                .AddSingleton<ILogFormatter, LogFormatter>()
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));
        }
    }
}
