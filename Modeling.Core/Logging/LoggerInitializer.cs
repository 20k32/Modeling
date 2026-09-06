using Serilog;

namespace Modeling.Core.Logging
{
    internal static class LoggerInitializer
    {
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.Debug()
              .CreateLogger();
        }
    }
}
