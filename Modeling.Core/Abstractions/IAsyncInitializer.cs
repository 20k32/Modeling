using System.Threading.Tasks;

namespace Modeling.Core.Abstractions
{
    public interface IAsyncInitializer
    {
        Task InitializeAsync();
    }
}
