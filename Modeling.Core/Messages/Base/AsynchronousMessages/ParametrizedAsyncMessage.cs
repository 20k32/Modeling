using CommunityToolkit.Mvvm.Messaging.Messages;
using Modeling.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Messages.Base.AsynchronousMessages
{
    public class ParametrizedAsyncMessage<T>(object sender, T value) : AsyncMessage(sender), IDefaultCheck where T : IDefaultCheck
    {
        T Value { get; init; } = value;
        public bool IsDefault() => Sender is null || Value.IsDefault();
    }
}
