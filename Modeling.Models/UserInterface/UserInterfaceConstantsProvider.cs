using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.Constants;

namespace Modeling.Models.UserInterface
{
    sealed class UserInterfaceConstantsProvider : ObservableObject, IUserInterfaceConstantsProvider
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
        const float MINIMUM_CANVAS_WIDTH = 100;
        const float MINIMUM_CANVAS_HEIGHT = 100;
        const float MAXIMUM_PIXELS_PER_CENTIMTER = 200;
        const float MINIMUM_PIXELS_PER_CENTIMETER = 1;

        public string ApplicationPopupRootName => POPUP_ROOT_CONTROL_NAME;
        public string DisplayMemberPath => DEFAULT_DISPLAY_MEMBER_PATH;
        public string NavigationFrameName => NAVIGATION_CONTAINER_CONTROL_NAME;
        public int AnimationTimeoutMilliseconds => DEFAULT_ANIMATION_TIMEOUT_MILLISECONDS;
        public float SlidersStepFrequency => DEFAULT_SLIDERS_STEP_FREQUENCY;
        public double MinimumTextBoxWidth => MINIMUM_TEXT_BOX_WIDTH;
        public float IncrementingStep => INCREMENTING_STEP;
        public int FractionalPartLength => FRACTIONAL_PART_LENGTH;
        public float LargeIncrementingStep => LARGE_INCREMENTING_STEP;

        public float MaximumCanvasWidth => DrawingConstants.MAXIMUM_CANVAS_SIZE_PIXELS;
        public float MaximumCanvasHeight => DrawingConstants.MAXIMUM_CANVAS_SIZE_PIXELS;

        public float MinimumCanvasWidth => MINIMUM_CANVAS_WIDTH;
        public float MinimumCanvasHeight => MINIMUM_CANVAS_HEIGHT;

        public float MaximumPixelsPerCentimeter => MAXIMUM_PIXELS_PER_CENTIMTER;
        public float MinimumPixelsPerCentimeter => MINIMUM_PIXELS_PER_CENTIMETER;
    }
}
