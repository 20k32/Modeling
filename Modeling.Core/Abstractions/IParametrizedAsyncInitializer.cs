using System.Threading.Tasks;

namespace Modeling.Core.Abstractions
{
    public interface IParametrizedAsyncInitializer<T>
    {
        Task InitializeAsync(T parameter);
    }
}
