namespace Modeling.PlatformHelpers.Providers.ApplicationPackage
{
    /// <summary>
    /// This interface is used because of some Windows api is available only for packaged apps.
    /// </summary>
    public interface IApplicationPackageChecker
    {
        bool CheckApplicationPackaged();
    }
}
