using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modeling.Core.Logging.Formatting;
using System;
using System.Runtime.CompilerServices;

namespace Modeling.Core.Logging
{
    public sealed class Logger
    {
        private static readonly Lazy<ILogger> IocLoggerInitializer = new(() => Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("Application"),
            isThreadSafe: true);

        private static ILogger _iocLogger => IocLoggerInitializer.Value;

        private static ILogFormatter _iocLogFormatter => Ioc.Default.GetRequiredService<ILogFormatter>();

        public static void Information(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var formattedMessage = _iocLogFormatter.FormatMessage(message, memberName, filePath, lineNumber);
            _iocLogger.LogInformation(formattedMessage);
        }

        public static void Information(Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var formattedMessage = _iocLogFormatter.FormatMessage(ex, memberName, filePath, lineNumber);
            _iocLogger.LogInformation(formattedMessage);
        }

        public static void Exception(Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var formattedMessage = _iocLogFormatter.FormatMessage(ex, memberName, filePath, lineNumber);
            _iocLogger.LogInformation(formattedMessage);
        }
    }
}
