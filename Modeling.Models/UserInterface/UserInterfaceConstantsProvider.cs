namespace Modeling.Models.UserInterface
{
    sealed class UserInterfaceConstantsProvider : IUserInterfaceConstantsProvider
    {
        const float DEFAULT_SLIDERS_STEP_FREQUENCY = 1f;
        const int DEFAULT_ANIMATION_TIMEOUT_MILLISECONDS = 50;
        const string POPUP_ROOT_CONTROL_NAME = "ModelingApplicationPopupRoot";
        const string DEFAULT_DISPLAY_MEMBER_PATH = "Value";
        const string NAVIGATION_CONTAINER_CONTROL_NAME = "ModelingApplicationNavigationContainer";
        const double MINIMUM_TEXT_BOX_WIDTH = 55;
        const float INCREMENTING_STEP = 0.01f;
        const int FRACTIONAL_PART_LENGTH = 3;
        const float LARGE_INCREMENTING_STEP = 1;

        public string ApplicationPopupRootName => POPUP_ROOT_CONTROL_NAME;
        public string DisplayMemberPath => DEFAULT_DISPLAY_MEMBER_PATH;
        public string NavigationFrameName => NAVIGATION_CONTAINER_CONTROL_NAME;
        public int AnimationTimeoutMilliseconds => DEFAULT_ANIMATION_TIMEOUT_MILLISECONDS;
        public float SlidersStepFrequency => DEFAULT_SLIDERS_STEP_FREQUENCY;
        public double MinimumTextBoxWidth => MINIMUM_TEXT_BOX_WIDTH;
        public float IncrementingStep => INCREMENTING_STEP;
        public int FractionalPartLength => FRACTIONAL_PART_LENGTH;
        public float LargeIncrementingStep => LARGE_INCREMENTING_STEP;
    }
}
