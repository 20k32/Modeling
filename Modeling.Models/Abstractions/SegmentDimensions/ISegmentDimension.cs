namespace Modeling.Models.Abstractions.SegmentDimensions
{
    public interface ISegmentDimension
    {
        public float TopLeftHorizontalSmallLineFirstLength { get; }
        public float TopLeftHorizontalSmallLineSecondLength { get; }

        public float TopRightHorizontalSmallLineFirstLength { get; }
        public float TopRightHorizontalSmallLineSecondLength { get; }

        public float BottomLeftHorizontalSmallLineFirstLength { get; }
        public float BottomLeftHorizontalSmallLineSecondLength { get; }

        public float BottomRightHorizontalSmallLineFirstLength { get; }
        public float BottomRightHorizontalSmallLineSecondLength { get; }

        public float TopLeftVerticalSmallLineLength { get; }
        public float CenterLeftVerticalLargeLineLength { get; }
        public float BottomLeftVerticalSmallLineLength { get; }

        public float TopRightVerticalSmallLineLength { get; }
        public float CenterRightVerticalLargeLineLength { get; }
        public float BottomRightVerticalSmallLineLength { get; }

        public float TopLeftHalfCircleDiameter { get; }
        public float BottomLeftHalfCircleDiameter { get; }
        public float TopRightHalfCircleDiameter { get; }
        public float BottomRightHalfCircleDiameter { get; }

        public float TopLeftSmallCircleDiameter { get; }
        public float BottomLeftSmallCircleDiameter { get; }
        public float TopRightSmallCircleDiameter { get; }
        public float BottomRightSmallCircleDiameter { get; }

        public float CenterSmallCircleDiameter { get; }
        public float CenterLargeCircleDiameter { get; }

        public float CenterLeftTopHorizontalLineLength { get; }
        public float CenterLeftBottomHorizontalLineLength { get; }
        public float CenterLeftVerticalLineLength { get; }

        public float CenterRightTopHorizontalLineLength { get; }
        public float CenterRightBottomHorizontalLineLength { get; }
        public float CenterRightVerticalLineLength { get; }

        public float TopLargeVerticalLineLength { get; }
        public float BottomLargeVerticalLineLength { get; }

        ISegmentDimension SetTopLeftHorizontalSmallLineFirstLength(float value);
        ISegmentDimension SetTopLeftHorizontalSmallLineSecondLength(float value);

        ISegmentDimension SetTopRightHorizontalSmallLineFirstLength(float value);
        ISegmentDimension SetTopRightHorizontalSmallLineSecondLength(float value);

        ISegmentDimension SetBottomLeftHorizontalSmallLineFirstLength(float value);
        ISegmentDimension SetBottomLeftHorizontalSmallLineSecondLength(float value);

        ISegmentDimension SetBottomRightHorizontalSmallLineFirstLength(float value);
        ISegmentDimension SetBottomRightHorizontalSmallLineSecondLength(float value);

        ISegmentDimension SetTopLeftVerticalSmallLineLength(float value);
        ISegmentDimension SetCenterLeftVerticalLargeLineLength(float value);
        ISegmentDimension SetBottomLeftVerticalSmallLineLength(float value);

        ISegmentDimension SetTopRightVerticalSmallLineLength(float value);
        ISegmentDimension SetCenterRightVerticalLargeLineLength(float value);
        ISegmentDimension SetBottomRightVerticalSmallLineLength(float value);

        ISegmentDimension SetTopLeftHalfCircleDiameter(float value);
        ISegmentDimension SetBottomLeftHalfCircleDiameter(float value);
        ISegmentDimension SetTopRightHalfCircleDiameter(float value);
        ISegmentDimension SetBottomRightHalfCircleDiameter(float value);

        ISegmentDimension SetTopLeftSmallCircleDiameter(float value);
        ISegmentDimension SetBottomLeftSmallCircleDiameter(float value);
        ISegmentDimension SetTopRightSmallCircleDiameter(float value);
        ISegmentDimension SetBottomRightSmallCircleDiameter(float value);

        ISegmentDimension SetCenterSmallCircleDiameter(float value);
        ISegmentDimension SetCenterLargeCircleDiameter(float value);

        ISegmentDimension SetCenterLeftTopHorizontalLineLength(float value);
        ISegmentDimension SetCenterLeftBottomHorizontalLineLength(float value);
        ISegmentDimension SetCenterLeftVerticalLineLength(float value);

        ISegmentDimension SetCenterRightTopHorizontalLineLength(float value);
        ISegmentDimension SetCenterRightBottomHorizontalLineLength(float value);
        ISegmentDimension SetCenterRightVerticalLineLength(float value);

        ISegmentDimension SetTopLargeVerticalLineLength(float value);
        ISegmentDimension SetBottomLargeVerticalLineLength(float value);
    }
}
