namespace Modeling.PlatformHelpers.Miscellaneous
{
    static class Constants
    {
        public const string ARGUMENT_NOT_NULL_EXCEPTION_FORMAT = "{0} must be null";
        public const string ARGUMENT_NULL_EXCEPTION_FORMAT = "{0} is null";

        public const string UNPACKAGED_APPLICATION_PUBLISHER = "Yehor";
        public const string UNPACKAGED_APPLICATION_PRODUCT_NAME = "Modeling";
        public const string UNPACKAGED_APPLICATION_SETTINGS_FOLDER_NAME = "Settings";

        //for unpackaged apps future access list simulation is used - app creates settings file in settings folder in %appdata%
        public const string UNPACKAGED_APPLICATION_FUTURE_ACCES_LIST_FILE_NAME_WITH_EXTENSION = "FutureAccessList.txt";
    }
}
