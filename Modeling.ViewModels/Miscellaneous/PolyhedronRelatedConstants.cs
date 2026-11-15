namespace Modeling.ViewModels.Miscellaneous
{
    static class PolyhedronRelatedConstants
    {
        public const float DEFAULT_SIDES_COUNT = 6;
        public const float MINIMUM_SIDES_COUNT = 3;
        public const float MAXIMUM_SIDES_COUNT = 12;

        public const float DEFAULT_RADIUS_CENTIMETERS = 1.5f;
        public const float DEFAULT_HEIGHT_CENTIMETERS = 3;
        public const float DEFAULT_ROOF_HEIGHT_CENTIMETERS = 1.5f;
        public const float MINIMUM_SIZE_CENTIMETERS = 0.2f;
        public const float MAXIMUM_SIZE_CENTIMETERS = 15;

        public const float DEFAULT_SCALE = 1;
        public const float MINIMUM_SCALE = 0.1f;
        public const float MAXIMUM_SCALE = 5;

        public const float DEFAULT_ROTATION_Z_DEGREES = 20;

        // the solid stands aside from the axes, so the edges along X visibly run to their vanishing point at the origin
        public const float DEFAULT_TRANSLATION_X_CENTIMETERS = -2;
        public const float DEFAULT_TRANSLATION_Y_CENTIMETERS = 2;
        public const float DEFAULT_TRANSLATION_Z_CENTIMETERS = 0;
        public const float MAXIMUM_TRANSLATION_CENTIMETERS = 20;

        // distances from the plane x = 0 to the centers of projection on the axes
        public const float DEFAULT_CENTER_X_CENTIMETERS = 30;
        public const float DEFAULT_CENTER_Y_CENTIMETERS = 20;
        public const float DEFAULT_CENTER_Z_CENTIMETERS = 18;
        public const float MINIMUM_CENTER_CENTIMETERS = 1;
        public const float MAXIMUM_CENTER_CENTIMETERS = 200;

        public const float DEFAULT_ANIMATION_HEIGHT_MINIMUM = 2;
        public const float DEFAULT_ANIMATION_HEIGHT_MAXIMUM = 5;
        public const float DEFAULT_ANIMATION_ROTATION_STEP_DEGREES = 3;
        public const float MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES = 45;
        public const float ANIMATION_HEIGHT_STEP_CENTIMETERS = 0.05f;

        public const float SPATIAL_AXIS_LENGTH_CENTIMETERS = 8;
        public const float SPATIAL_AXIS_CENTER_RATIO = 0.8f;
        public const float OWN_AXIS_LENGTH_RATIO = 1.4f;
        public const float POINT_MARKER_RADIUS_PIXELS = 4;

        public const string NUMBER_FORMAT = "0.###";
    }
}
