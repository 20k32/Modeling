using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.ViewModels;

namespace Modeling.UI.DependencyInjection
{
    internal sealed class ServiceLocatorWrapper
    {
        public ServiceLocatorWrapper()
        { }

        public static DrawingViewModel DrawingViewModel => Ioc.Default.GetRequiredService<DrawingViewModel>();
    }
}
