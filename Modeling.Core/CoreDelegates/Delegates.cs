
using System.Threading.Tasks;

namespace Modeling.Core.CoreDelegates
{
    public delegate void ActionEventHandler();
    public delegate void ActionEventHandler<T>(T value);
    public delegate T FuncEventHandler<T>(T value);
    public delegate Tresult FuncEventHandler<Tparameter, Tresult>(Tparameter value);
    public delegate Task AsyncActionEventHandler();
    public delegate Task AsyncActionEventHandler<T>(T value);
    public delegate Task<T> AsyncFuncEventHandler<T>(out T value);
    public delegate Task<Tout> AsyncFuncEventHandler<Tin, Tout>(in Tin inValue, out Tout outValue);
}
