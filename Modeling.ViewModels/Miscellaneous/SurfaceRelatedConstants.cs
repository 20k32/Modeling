namespace Modeling.ViewModels.Miscellaneous
{
    static class SurfaceRelatedConstants
    {
        public const float DEFAULT_RADIUS_CENTIMETERS = 3;
        public const float MINIMUM_RADIUS_CENTIMETERS = 0.2f;
        public const float MAXIMUM_RADIUS_CENTIMETERS = 15;

        // k in z = a − k·zш for the upper (concave) part, the variant uses 0.5
        public const float DEFAULT_DEPTH_COEFFICIENT = 0.5f;
        public const float MINIMUM_DEPTH_COEFFICIENT = 0;
        public const float MAXIMUM_DEPTH_COEFFICIENT = 2;

        public const float DEFAULT_MESH_STEP_DEGREES = 10;
        public const float MINIMUM_MESH_STEP_DEGREES = 2;
        public const float MAXIMUM_MESH_STEP_DEGREES = 45;

        public const float DEFAULT_TRANSLATION_X_CENTIMETERS = 0;
        public const float DEFAULT_TRANSLATION_Y_CENTIMETERS = -5;
        public const float DEFAULT_TRANSLATION_Z_CENTIMETERS = 1;
        public const float DEFAULT_ROTATION_Z_DEGREES = 0;

        public const float DEFAULT_CENTER_X_CENTIMETERS = 25;
        public const float DEFAULT_CENTER_Y_CENTIMETERS = 40;
        public const float DEFAULT_CENTER_Z_CENTIMETERS = 40;

        // the contour is placed on the lower (spherical) part, facing the center of projection on X
        public const float CONTOUR_BASE_U_DEGREES = 0;
        public const float CONTOUR_BASE_V_DEGREES = -38;
        public const float DEFAULT_CONTOUR_SCALE_DEGREES_PER_CENTIMETER = 9;
        public const float MINIMUM_CONTOUR_SCALE = 0.5f;
        public const float MAXIMUM_CONTOUR_SCALE = 40;
        public const float MAXIMUM_CONTOUR_SHIFT_DEGREES = 360;
        public const int CONTOUR_ARC_POINTS_COUNT = 12;

        public const float DEFAULT_ANIMATION_RADIUS_MINIMUM = 2.5f;
        public const float DEFAULT_ANIMATION_RADIUS_MAXIMUM = 3.5f;
        public const float DEFAULT_ANIMATION_DEPTH_MINIMUM = 0.1f;
        public const float DEFAULT_ANIMATION_DEPTH_MAXIMUM = 1.5f;
        public const float ANIMATION_RADIUS_STEP_CENTIMETERS = 0.02f;
        public const float ANIMATION_DEPTH_STEP = 0.02f;

        public const string NUMBER_FORMAT = "0.###";
    }
}
