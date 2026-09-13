using Modeling.Core.Abstractions;
using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Base.SynchronousMessages;

namespace Modeling.Core.Extensions
{
    public static class MessageExtensions
    {

        public static bool ApplyBasicMessageValidation(this Message message, object sender) =>
            message is not null && sender is not null && !message.IsDefault();

        public static bool ApplyBasicMessageValidation<T>(this ParametrizedMessage<T> message, object sender)
            where T : IDefaultCheck => message is not null
            && sender is not null
            && !message.IsDefault();

        public static bool ApplyBasicMessageValidation(this AsyncMessage message, object sender) =>
            message is not null && sender is not null && !message.IsDefault();

        public static bool ApplyBasicMessageValidation<T>(this ParametrizedAsyncMessage<T> message, object sender)
            where T : IDefaultCheck => message is not null
            && sender is not null
            && !message.IsDefault();

        public static bool ApplyBasicMessageValidation<TParameter, TResult>(this ParametrizedAsyncMessageReturnValue<TParameter, TResult> message, object sender)
            where TParameter : IDefaultCheck => message is not null
            && sender is not null
            && !message.IsDefault();
    }
}
