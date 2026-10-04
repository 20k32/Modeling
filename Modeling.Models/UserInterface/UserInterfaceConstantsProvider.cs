namespace Modeling.Models.UserInterface
{
    sealed class UserInterfaceConstantsProvider : IUserInterfaceConstantsProvider
    {
        const float DEFAULT_SLIDERS_STEP_FREQUENCY = 0.01f;
        const int DEFAULT_ANIMATION_TIMEOUT_MILLISECONDS = 50;
        const string POPUP_ROOT_CONTROL_NAME = "ModelingApplicationPopupRoot";
        const string DEFAULT_DISPLAY_MEMBER_PATH = "Value";
        const string NAVIGATION_CONTAINER_CONTROL_NAME = "ModelingApplicationNavigationContainer";

        public string ApplicationPopupRootName => POPUP_ROOT_CONTROL_NAME;
        public string DisplayMemberPath => DEFAULT_DISPLAY_MEMBER_PATH;
        public string NavigationFrameName => NAVIGATION_CONTAINER_CONTROL_NAME;
        public int AnimationTimeoutMilliseconds => DEFAULT_ANIMATION_TIMEOUT_MILLISECONDS;
        public float SlidersStepFrequency => DEFAULT_SLIDERS_STEP_FREQUENCY;
    }
}
