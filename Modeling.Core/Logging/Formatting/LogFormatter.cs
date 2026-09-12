using Modeling.Core.Messages.Base.SynchronousMessages;
using System;
using System.Text;

namespace Modeling.Core.Logging.Formatting
{
    class LogFormatter : ILogFormatter
    {
        const string MESSAGE_FORMAT = "Message: {0}";
        const string METHOD_NAME_FORMAT = "Method name: {0}";
        const string FILE_PATH_FORMAT = "File path: {0}";
        const string LINE_NUMBER_FORMAT = "Line number: {0}";

        const string PROPERTY_VALUE_FORMAT = "{0}: {1}";
        const string EXCETPION_EXTRA_DATA_FORMAT = "Data[{0}]: {1}";

        static void AppendExceptionFields(StringBuilder builder, System.Exception ex)
        {
            var exceptionType = ex.GetType();
            var properties = exceptionType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                try
                {
                    var propertyValue = property.GetValue(ex);
                    var formattedString = string.Format(PROPERTY_VALUE_FORMAT, propertyName, propertyValue);
                    builder.AppendLine(formattedString);
                }
                catch
                {
                    builder.AppendLine($"Error receiving exception property: {propertyName}");
                }
            }

            foreach (var key in ex.Data.Keys)
            {
                var formattedString = string.Format(EXCETPION_EXTRA_DATA_FORMAT, key, ex.Data[key]?.ToString() ?? "empty_value");
                builder.AppendLine(formattedString);
            }
        }

        static void AppendCommonInfo(StringBuilder builder, string memberName, string filePath, int lineNumber)
        {
            builder.AppendLine(string.Format(METHOD_NAME_FORMAT, memberName));
            builder.AppendLine(string.Format(FILE_PATH_FORMAT, filePath));
            builder.AppendLine(string.Format(LINE_NUMBER_FORMAT, lineNumber));
        }


        public string FormatMessage(string message, string memberName, string filePath, int lineNumber)
        {
            var builder = new StringBuilder();

            builder.AppendLine(string.Format(MESSAGE_FORMAT, message));
            AppendCommonInfo(builder, memberName, filePath, lineNumber);

            return builder.ToString();
        }

        public string FormatMessage(Exception ex, string memberName, string filePath, int lineNumber)
        {
            var builder = new StringBuilder();

            AppendExceptionFields(builder, ex);
            AppendCommonInfo(builder, memberName, filePath, lineNumber);

            return builder.ToString();
        }
    }
}
