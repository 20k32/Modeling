using Windows.ApplicationModel;

namespace Modeling.PlatformHelpers.Providers.ApplicationPackage
{
    sealed class ApplicationPackageChecker : IApplicationPackageChecker
    {
        public bool CheckApplicationPackaged()
        {
            var result = false;

            try
            {
                _ = Package.Current;
                result = true;
            }
            catch 
            { }

            return result;
        }
    }
}
