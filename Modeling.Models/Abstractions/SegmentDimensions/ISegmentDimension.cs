namespace Modeling.Models.Abstractions.SegmentDiemnsions
{
    public interface ISegmentDimension
    {
        // Circles
        public float TopLeftHalfCircleDiameter { get; }
        public float TopLeftInnerCircleDiameter { get; }
        public float BottomLeftInnerCircleDiameter { get; }
        public float BottomLeftHalfCircleDiameter { get; }

        public float LargeCircleDiameter { get; }
        public float SmallCircleDiameter { get; }

        public float TopRightInnerCircleDiameter { get; }
        public float TopRightHalfCircleDiameter { get; }

        public float BottomRightHalfCircleDiameter { get; }
        public float BottomRightInnerCircleDiameter { get; }

        // Lines
        public float TopLeftHorizontalLineLength { get; }
        public float BottomHorizontalLineLength { get; }

        public float LeftUpperVerticalLineLength { get; }
        public float LeftMiddleVerticalLineLength { get; }
        public float LeftLowerVerticalLineLength { get; }

        public float LeftSquareVerticalLineLength { get; }
        public float LeftSquareHorizontalLineLength { get; }

        public float RightSquareVerticalLineLength { get; }
        public float RightSquareHorizontalLineLength { get; }

        public float BottomSquareHorizontalLineLength { get; }
        public float BottomLeftSquareHorizontalLineLength { get; }

        public float TopRightConnectionLineLength { get; }
        public float TopRightLowerConnectionLineLength { get; }
        public float TopRightVerticalLineLength { get; }

        public float BottomRightConnectionLineLength { get; }

        public float TopLeftCircleConnectionLineLength { get; }
        public float RightMiddleVerticalLineLength { get; }
        public float BottomRightVerticalLineLength { get; }

        public float TopTopLeftHalfCircleTopLineLength { get; }
        public float TopBottomLeftHalfCircleTopLineLength { get; }
        public float BottomTopLeftHalfCircleTopLineLength { get; }
        public float BottomBottomLeftHalfCircleTopLineLength { get; }

        ISegmentDimension SetTopLeftHalfCircleDiameter(float value);
        ISegmentDimension SetTopLeftInnerCircleDiameter(float value);
        ISegmentDimension SetBottomLeftInnerCircleDiameter(float value);
        ISegmentDimension SetBottomLeftHalfCircleDiameter(float value);

        ISegmentDimension SetLargeCircleDiameter(float value);
        ISegmentDimension SetSmallCircleDiameter(float value);

        ISegmentDimension SetTopRightInnerCircleDiameter(float value);
        ISegmentDimension SetTopRightHalfCircleDiameter(float value);

        ISegmentDimension SetBottomRightHalfCircleDiameter(float value);
        ISegmentDimension SetBottomRightInnerCircleDiameter(float value);

        ISegmentDimension SetTopHorizontalLineLength(float value);
        ISegmentDimension SetBottomHorizontalLineLength(float value);

        ISegmentDimension SetLeftUpperVerticalLineLength(float value);
        ISegmentDimension SetLeftMiddleVerticalLineLength(float value);
        ISegmentDimension SetLeftLowerVerticalLineLength(float value);

        ISegmentDimension SetLeftSquareVerticalLineLength(float value);
        ISegmentDimension SetLeftSquareHorizontalLineLength(float value);

        ISegmentDimension SetRightSquareVerticalLineLength(float value);
        ISegmentDimension SetRightSquareHorizontalLineLength(float value);

        ISegmentDimension SetBottomSquareHorizontalLineLength(float value);
        ISegmentDimension SetBottomLeftSquareHorizontalLineLength(float value);

        ISegmentDimension SetTopRightConnectionLineLength(float value);
        ISegmentDimension SetTopRightLowerConnectionLineLength(float value);
        ISegmentDimension SetTopRightVerticalLineLength(float value);

        ISegmentDimension SetBottomRightConnectionLineLength(float value);

        ISegmentDimension SetTopLeftCircleConnectionLineLength(float value);
        ISegmentDimension SetRightMiddleVerticalLineLength(float value);
        ISegmentDimension SetBottomRightVerticalLineLength(float value);

        ISegmentDimension SetTopLeftHalfCircleTopLineLength(float value);
        ISegmentDimension SetBottomLeftHalfCircleTopLineLength(float value);
        ISegmentDimension SetBottomTopLeftHalfCircleTopLineLength(float value);
        ISegmentDimension SetBottomBottomLeftHalfCircleTopLineLength(float value);
    }
}
