namespace Modeling.Models.UserInterface
{
    sealed class UserInterfaceConstantsProvider : IUserInterfaceConstantsProvider
    {
        const string POPUP_ROOT_CONTROL_NAME = "ModelingApplicationPopupRoot";
        const string DEFAULT_DISPLAY_MEMBER_PATH = "Value";

        public string ApplicationPopupRootName => POPUP_ROOT_CONTROL_NAME;
        public string DisplayMemberPath => DEFAULT_DISPLAY_MEMBER_PATH;
    }
}
