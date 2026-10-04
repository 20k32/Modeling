using Modeling.Models.Abstractions.SegmentDiemnsions;

namespace Modeling.Models.Drawing
{
    sealed class SegmentDimension : ISegmentDimension
    {
        public float TopLeftHalfCircleDiameter { get; private set; }
        public float TopLeftInnerCircleDiameter { get; private set; }
        public float BottomLeftInnerCircleDiameter { get; private set; }
        public float BottomLeftHalfCircleDiameter { get; private set; }

        public float LargeCircleDiameter { get; private set; }
        public float SmallCircleDiameter { get; private set; }

        public float TopRightInnerCircleDiameter { get; private set; }
        public float TopRightHalfCircleDiameter { get; private set; }

        public float BottomRightHalfCircleDiameter { get; private set; }
        public float BottomRightInnerCircleDiameter { get; private set; }

        public float TopLeftHorizontalLineLength { get; private set; }
        public float BottomHorizontalLineLength { get; private set; }

        public float LeftUpperVerticalLineLength { get; private set; }
        public float LeftMiddleVerticalLineLength { get; private set; }
        public float LeftLowerVerticalLineLength { get; private set; }

        public float LeftSquareVerticalLineLength { get; private set; }
        public float LeftSquareHorizontalLineLength { get; private set; }

        public float RightSquareVerticalLineLength { get; private set; }
        public float RightSquareHorizontalLineLength { get; private set; }

        public float BottomSquareHorizontalLineLength { get; private set; }
        public float BottomLeftSquareHorizontalLineLength { get; private set; }

        public float TopRightConnectionLineLength { get; private set; }
        public float TopRightLowerConnectionLineLength { get; private set; }
        public float TopRightVerticalLineLength { get; private set; }

        public float BottomRightConnectionLineLength { get; private set; }

        public float TopLeftCircleConnectionLineLength { get; private set; }
        public float RightMiddleVerticalLineLength { get; private set; }
        public float BottomRightVerticalLineLength { get; private set; }

        public float TopTopLeftHalfCircleTopLineLength { get; private set; }
        public float TopBottomLeftHalfCircleTopLineLength { get; private set; }
        public float BottomTopLeftHalfCircleTopLineLength { get; private set; }
        public float BottomBottomLeftHalfCircleTopLineLength { get; private set; }

        public ISegmentDimension SetTopLeftHalfCircleDiameter(float value) { TopLeftHalfCircleDiameter = value; return this; }
        public ISegmentDimension SetTopLeftInnerCircleDiameter(float value) { TopLeftInnerCircleDiameter = value; return this; }
        public ISegmentDimension SetBottomLeftInnerCircleDiameter(float value) { BottomLeftInnerCircleDiameter = value; return this; }
        public ISegmentDimension SetBottomLeftHalfCircleDiameter(float value) { BottomLeftHalfCircleDiameter = value; return this; }
        public ISegmentDimension SetLargeCircleDiameter(float value) { LargeCircleDiameter = value; return this; }
        public ISegmentDimension SetSmallCircleDiameter(float value) { SmallCircleDiameter = value; return this; }
        public ISegmentDimension SetTopRightInnerCircleDiameter(float value) { TopRightInnerCircleDiameter = value; return this; }
        public ISegmentDimension SetTopRightHalfCircleDiameter(float value) { TopRightHalfCircleDiameter = value; return this; }
        public ISegmentDimension SetBottomRightHalfCircleDiameter(float value) { BottomRightHalfCircleDiameter = value; return this; }
        public ISegmentDimension SetBottomRightInnerCircleDiameter(float value) { BottomRightInnerCircleDiameter = value; return this; }
        public ISegmentDimension SetTopHorizontalLineLength(float value) { TopLeftHorizontalLineLength = value; return this; }
        public ISegmentDimension SetBottomHorizontalLineLength(float value) { BottomHorizontalLineLength = value; return this; }
        public ISegmentDimension SetLeftUpperVerticalLineLength(float value) { LeftUpperVerticalLineLength = value; return this; }
        public ISegmentDimension SetLeftMiddleVerticalLineLength(float value) { LeftMiddleVerticalLineLength = value; return this; }
        public ISegmentDimension SetLeftLowerVerticalLineLength(float value) { LeftLowerVerticalLineLength = value; return this; }
        public ISegmentDimension SetLeftSquareVerticalLineLength(float value) { LeftSquareVerticalLineLength = value; return this; }
        public ISegmentDimension SetLeftSquareHorizontalLineLength(float value) { LeftSquareHorizontalLineLength = value; return this; }
        public ISegmentDimension SetRightSquareVerticalLineLength(float value) { RightSquareVerticalLineLength = value; return this; }
        public ISegmentDimension SetRightSquareHorizontalLineLength(float value) { RightSquareHorizontalLineLength = value; return this; }
        public ISegmentDimension SetBottomSquareHorizontalLineLength(float value) { BottomSquareHorizontalLineLength = value; return this; }
        public ISegmentDimension SetBottomLeftSquareHorizontalLineLength(float value) { BottomLeftSquareHorizontalLineLength = value; return this; }
        public ISegmentDimension SetTopRightConnectionLineLength(float value) { TopRightConnectionLineLength = value; return this; }
        public ISegmentDimension SetTopRightLowerConnectionLineLength(float value) { TopRightLowerConnectionLineLength = value; return this; }
        public ISegmentDimension SetTopRightVerticalLineLength(float value) { TopRightVerticalLineLength = value; return this; }
        public ISegmentDimension SetBottomRightConnectionLineLength(float value) { BottomRightConnectionLineLength = value; return this; }
        public ISegmentDimension SetTopLeftCircleConnectionLineLength(float value) { TopLeftCircleConnectionLineLength = value; return this; }
        public ISegmentDimension SetRightMiddleVerticalLineLength(float value) { RightMiddleVerticalLineLength = value; return this; }
        public ISegmentDimension SetBottomRightVerticalLineLength(float value) { BottomRightVerticalLineLength = value; return this; }
        public ISegmentDimension SetTopLeftHalfCircleTopLineLength(float value) { TopTopLeftHalfCircleTopLineLength = value; return this; }
        public ISegmentDimension SetBottomLeftHalfCircleTopLineLength(float value) { TopBottomLeftHalfCircleTopLineLength = value; return this; }
        public ISegmentDimension SetBottomTopLeftHalfCircleTopLineLength(float value) { BottomTopLeftHalfCircleTopLineLength = value; return this; }
        public ISegmentDimension SetBottomBottomLeftHalfCircleTopLineLength(float value) { BottomBottomLeftHalfCircleTopLineLength = value; return this; }
    }
}
