
using System.Threading.Tasks;

namespace Modeling.Core.CoreDelegates
{
    public delegate void ActionEventHandler();
    public delegate T ActionEventHandler<T>(T value);
    public delegate Tresult ActionEventHandler<Tparameter, Tresult>(Tparameter value);
    public delegate Task<T> AsyncActionEventHandler<T>(out T value);
    public delegate Task<Tout> AsyncActionEventHandler<Tin, Tout>(in Tin inValue, out Tout outValue);
}
