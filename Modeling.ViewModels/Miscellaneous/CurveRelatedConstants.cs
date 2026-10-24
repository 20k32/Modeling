namespace Modeling.ViewModels.Miscellaneous
{
    static class CurveRelatedConstants
    {
        public const float DEFAULT_SCALE_CENTIMETERS = 4;
        public const float MINIMUM_SCALE_CENTIMETERS = 0.1f;
        public const float MAXIMUM_SCALE_CENTIMETERS = 50;

        public const float DEFAULT_POINTS_COUNT = 720;
        public const float MINIMUM_POINTS_COUNT = 16;
        public const float MAXIMUM_POINTS_COUNT = 10000;

        public const float DEFAULT_POINT_PARAMETER_DEGREES = 30;
        public const float DEFAULT_TANGENT_LENGTH_CENTIMETERS = 4;
        public const float MINIMUM_TANGENT_LENGTH_CENTIMETERS = 0.1f;

        public const float ANIMATION_MINIMUM_SCALE_CENTIMETERS = 1;
        public const float ANIMATION_MAXIMUM_SCALE_CENTIMETERS = 5;
        public const float ANIMATION_SCALE_STEP_CENTIMETERS = 0.05f;
        public const float ANIMATION_POINT_PARAMETER_STEP_DEGREES = 2;

        public const float POINT_MARKER_RADIUS_PIXELS = 5;
        public const float ROTATION_POINT_MARKER_SIZE_PIXELS = 8;
        public const float DRAGGING_HIT_DISTANCE_PIXELS = 10;

        public const int NEAREST_POINT_REFINEMENT_STEPS = 32;

        public const string NUMBER_FORMAT = "0.###";
    }
}
