using Modeling.Models.Abstractions.SegmentDimensions;

namespace Modeling.Models.Drawing
{
    sealed class SegmentDimension : ISegmentDimension
    {
        public float TopLeftHorizontalSmallLineFirstLength { get; private set; }
        public float TopLeftHorizontalSmallLineSecondLength { get; private set; }

        public float TopRightHorizontalSmallLineFirstLength { get; private set; }
        public float TopRightHorizontalSmallLineSecondLength { get; private set; }

        public float BottomLeftHorizontalSmallLineFirstLength { get; private set; }
        public float BottomLeftHorizontalSmallLineSecondLength { get; private set; }

        public float BottomRightHorizontalSmallLineFirstLength { get; private set; }
        public float BottomRightHorizontalSmallLineSecondLength { get; private set; }

        public float TopLeftVerticalSmallLineLength { get; private set; }
        public float CenterLeftVerticalLargeLineLength { get; private set; }
        public float BottomLeftVerticalSmallLineLength { get; private set; }

        public float TopRightVerticalSmallLineLength { get; private set; }
        public float CenterRightVerticalLargeLineLength { get; private set; }
        public float BottomRightVerticalSmallLineLength { get; private set; }

        public float TopLeftHalfCircleDiameter { get; private set; }
        public float BottomLeftHalfCircleDiameter { get; private set; }
        public float TopRightHalfCircleDiameter { get; private set; }
        public float BottomRightHalfCircleDiameter { get; private set; }

        public float TopLeftSmallCircleDiameter { get; private set; }
        public float BottomLeftSmallCircleDiameter { get; private set; }
        public float TopRightSmallCircleDiameter { get; private set; }
        public float BottomRightSmallCircleDiameter { get; private set; }

        public float CenterSmallCircleDiameter { get; private set; }
        public float CenterLargeCircleDiameter { get; private set; }

        public float CenterLeftTopHorizontalLineLength { get; private set; }
        public float CenterLeftBottomHorizontalLineLength { get; private set; }
        public float CenterLeftVerticalLineLength { get; private set; }

        public float CenterRightTopHorizontalLineLength { get; private set; }
        public float CenterRightBottomHorizontalLineLength { get; private set; }
        public float CenterRightVerticalLineLength { get; private set; }

        public float TopLargeVerticalLineLength { get; private set; }
        public float BottomLargeVerticalLineLength { get; private set; }

        public ISegmentDimension SetTopLeftHorizontalSmallLineFirstLength(float value)
        {
            TopLeftHorizontalSmallLineFirstLength = value;
            return this;
        }

        public ISegmentDimension SetTopLeftHorizontalSmallLineSecondLength(float value)
        {
            TopLeftHorizontalSmallLineSecondLength = value;
            return this;
        }

        public ISegmentDimension SetTopRightHorizontalSmallLineFirstLength(float value)
        {
            TopRightHorizontalSmallLineFirstLength = value;
            return this;
        }

        public ISegmentDimension SetTopRightHorizontalSmallLineSecondLength(float value)
        {
            TopRightHorizontalSmallLineSecondLength = value;
            return this;
        }

        public ISegmentDimension SetBottomLeftHorizontalSmallLineFirstLength(float value)
        {
            BottomLeftHorizontalSmallLineFirstLength = value;
            return this;
        }

        public ISegmentDimension SetBottomLeftHorizontalSmallLineSecondLength(float value)
        {
            BottomLeftHorizontalSmallLineSecondLength = value;
            return this;
        }

        public ISegmentDimension SetBottomRightHorizontalSmallLineFirstLength(float value)
        {
            BottomRightHorizontalSmallLineFirstLength = value;
            return this;
        }

        public ISegmentDimension SetBottomRightHorizontalSmallLineSecondLength(float value)
        {
            BottomRightHorizontalSmallLineSecondLength = value;
            return this;
        }

        public ISegmentDimension SetTopLeftVerticalSmallLineLength(float value)
        {
            TopLeftVerticalSmallLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterLeftVerticalLargeLineLength(float value)
        {
            CenterLeftVerticalLargeLineLength = value;
            return this;
        }

        public ISegmentDimension SetBottomLeftVerticalSmallLineLength(float value)
        {
            BottomLeftVerticalSmallLineLength = value;
            return this;
        }

        public ISegmentDimension SetTopRightVerticalSmallLineLength(float value)
        {
            TopRightVerticalSmallLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterRightVerticalLargeLineLength(float value)
        {
            CenterRightVerticalLargeLineLength = value;
            return this;
        }

        public ISegmentDimension SetBottomRightVerticalSmallLineLength(float value)
        {
            BottomRightVerticalSmallLineLength = value;
            return this;
        }

        public ISegmentDimension SetTopLeftHalfCircleDiameter(float value)
        {
            TopLeftHalfCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetBottomLeftHalfCircleDiameter(float value)
        {
            BottomLeftHalfCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetTopRightHalfCircleDiameter(float value)
        {
            TopRightHalfCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetBottomRightHalfCircleDiameter(float value)
        {
            BottomRightHalfCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetTopLeftSmallCircleDiameter(float value)
        {
            TopLeftSmallCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetBottomLeftSmallCircleDiameter(float value)
        {
            BottomLeftSmallCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetTopRightSmallCircleDiameter(float value)
        {
            TopRightSmallCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetBottomRightSmallCircleDiameter(float value)
        {
            BottomRightSmallCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetCenterSmallCircleDiameter(float value)
        {
            CenterSmallCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetCenterLargeCircleDiameter(float value)
        {
            CenterLargeCircleDiameter = value;
            return this;
        }

        public ISegmentDimension SetCenterLeftTopHorizontalLineLength(float value)
        {
            CenterLeftTopHorizontalLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterLeftBottomHorizontalLineLength(float value)
        {
            CenterLeftBottomHorizontalLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterLeftVerticalLineLength(float value)
        {
            CenterLeftVerticalLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterRightTopHorizontalLineLength(float value)
        {
            CenterRightTopHorizontalLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterRightBottomHorizontalLineLength(float value)
        {
            CenterRightBottomHorizontalLineLength = value;
            return this;
        }

        public ISegmentDimension SetCenterRightVerticalLineLength(float value)
        {
            CenterRightVerticalLineLength = value;
            return this;
        }

        public ISegmentDimension SetTopLargeVerticalLineLength(float value)
        {
            TopLargeVerticalLineLength = value;
            return this;
        }

        public ISegmentDimension SetBottomLargeVerticalLineLength(float value)
        {
            BottomLargeVerticalLineLength = value;
            return this;
        }
    }
}
