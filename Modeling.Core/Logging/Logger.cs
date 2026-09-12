using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modeling.Core.Logging.Formatting;
using System;
using System.Runtime.CompilerServices;

namespace Modeling.Core.Logging
{
    public sealed class Logger
    {
        static readonly Lazy<ILogger> IocLoggerInitializer = new(() => Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("Application"),
            isThreadSafe: true);

        static ILogger IocLogger => IocLoggerInitializer.Value;

        static ILogFormatter IocLogFormatter => Ioc.Default.GetRequiredService<ILogFormatter>();

        public static void Information(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var formattedMessage = IocLogFormatter.FormatMessage(message, memberName, filePath, lineNumber);
            IocLogger.LogInformation(formattedMessage);
        }

        public static void Information(Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var formattedMessage = IocLogFormatter.FormatMessage(ex, memberName, filePath, lineNumber);
            IocLogger.LogInformation(formattedMessage);
        }

        public static void Exception(Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var formattedMessage = IocLogFormatter.FormatMessage(ex, memberName, filePath, lineNumber);
            IocLogger.LogInformation(formattedMessage);
        }

        public static void LoadedInformation(string page) => Information($"{page} loaded.");
        public static void InitializedInformation(string component) => Information($"{component} initialized.");

    }
}
