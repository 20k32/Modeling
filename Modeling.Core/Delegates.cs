using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core
{
    public delegate void ActionEventHandler();
    public delegate T ActionEventHandler<T>(T value);
    public delegate Task<T> AsyncActionEventHandler<T>(out T value);
    public delegate Task<Tout> AsyncActionEventHandler<Tin, Tout>(in Tin inValue, out Tout outValue);
}
