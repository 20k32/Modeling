using Modeling.Core.Abstractions;
using Modeling.Core.Messages.Base.SynchronousMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Extensions
{
    public static class MessageExtensions
    {
        public static bool ApplyBaseMessageValidation(this Message message) =>
            message is not null && !message.IsDefault();

        public static bool ApplyBaseMessageValidation(this Message message, object sender) =>
            message is not null && sender is not null && !message.IsDefault();

        public static bool ApplyBaseMessageValidation<T>(this ParametrizedMessage<T> message, object sender)
            where T : IDefaultCheck => message is not null
            && sender is not null
            && !message.IsDefault();
    }
}
