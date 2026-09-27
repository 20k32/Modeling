namespace Modeling.Models.UserInterface
{
    sealed class UserInterfaceConstantsProvider : IUserInterfaceConstantsProvider
    {
        const string POPUP_ROOT_CONTROL_NAME = "ModelingApplicationPopupRoot";

        public string ApplicationPopupRootName => POPUP_ROOT_CONTROL_NAME;
    }
}
