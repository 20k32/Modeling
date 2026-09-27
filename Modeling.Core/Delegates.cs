using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core
{
    public delegate Action ActionEventHandler();
    public delegate Action<T> ActionEventHandler<T>(T value);
    public delegate Func<T> AsyncActionEventHandler<T>(out T value);
    public delegate Func<Tin, Tout> AsyncActionEventHandler<Tin, Tout>(in Tin inValue, out Tout outValue);
}
