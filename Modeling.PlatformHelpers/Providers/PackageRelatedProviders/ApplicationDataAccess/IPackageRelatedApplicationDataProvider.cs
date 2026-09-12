using Modeling.PlatformHelpers.Providers.ApplicationData;

namespace Modeling.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess
{
    interface IPackageRelatedApplicationDataProvider : IApplicationDataProvider
    {
        void Remove(string key);
        bool ContainsKey(string key);
    }
}
