using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Models.Drawing.Curves;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    public sealed partial class DrawingViewModel : BaseViewModel
    {
        [ObservableProperty]
        bool _pickShapeForResizing;

        [ObservableProperty]
        bool _changeFigurePosition;

        [ObservableProperty]
        bool _pickButtonsVisible;

        [ObservableProperty]
        bool _sizeEditingPanelVisible;

        [ObservableProperty]
        bool _positionEditingPanelVisible;

        [ObservableProperty]
        bool _cancelButtonVisible;

        [ObservableProperty]
        bool _lineEditingPanelVisible;

        [ObservableProperty]
        bool _circleEditingPanelVisible;

        [ObservableProperty]
        bool _positionEditingControlVisible;

        [ObservableProperty]
        bool _animateRotation;

        [ObservableProperty]
        bool _rotationPointVisible;

        [ObservableProperty]
        IPointGeometry _nearestSegment;

        [ObservableProperty]
        float _dimensionLengthCentimeters;

        [ObservableProperty]
        float _rotationAngle;

        [ObservableProperty]
        float _affineStartPointX;

        [ObservableProperty]
        float _affineStartPointY;

        [ObservableProperty]
        float _affineNewXPointX;

        [ObservableProperty]
        float _affineNewXPointY;

        [ObservableProperty]
        float _affineNewYPointX;

        [ObservableProperty]
        float _affineNewYPointY;

        [ObservableProperty]
        float _horizontalFigurePosition;

        [ObservableProperty]
        float _verticalFigurePosition;

        [ObservableProperty]
        float _horizontalRotationPointCenter;

        [ObservableProperty]
        float _verticalRotationPointCenter;

        [ObservableProperty]
        float _projectiveStartPointX;

        [ObservableProperty]
        float _projectiveStartPointY;

        [ObservableProperty]
        float _projectiveNewXPointX;

        [ObservableProperty]
        float _projectiveNewXPointY;

        [ObservableProperty]
        float _projectiveNewYPointX;

        [ObservableProperty]
        float _projectiveNewYPointY;

        [ObservableProperty]
        float _projectiveNewXWeight;

        [ObservableProperty]
        float _projectiveNewYWeight;

        [ObservableProperty]
        float _projectiveStartPointWeight;

        [ObservableProperty]
        float _curveScale;

        [ObservableProperty]
        float _curvePointsCount;

        [ObservableProperty]
        float _curvePointParameterDegrees;

        [ObservableProperty]
        float _curveTangentLength;

        [ObservableProperty]
        bool _curveTangentVisible = true;

        [ObservableProperty]
        bool _curveInflectionPointsVisible = true;

        [ObservableProperty]
        bool _curveRotationPointVisible = true;

        [ObservableProperty]
        bool _pickCurvePointByMouse;

        [ObservableProperty]
        bool _moveCurvePointDuringAnimation = true;

        [ObservableProperty]
        bool _isCurveAnimationRunning;

        [ObservableProperty]
        float _curveTranslationX;

        [ObservableProperty]
        float _curveTranslationY;

        [ObservableProperty]
        float _curveRotationPointX;

        [ObservableProperty]
        float _curveRotationPointY;

        [ObservableProperty]
        float _curveRotationAngle;

        [ObservableProperty]
        string _curvePointCoordinatesText;

        [ObservableProperty]
        string _curveTransformedPointCoordinatesText;

        [ObservableProperty]
        string _curveTangentSlopeText;

        [ObservableProperty]
        string _curveRadiusOfCurvatureText;

        [ObservableProperty]
        string _curveArcLengthToPointText;

        [ObservableProperty]
        string _curveTotalArcLengthText;

        [ObservableProperty]
        string _curveLoopArcLengthText;

        [ObservableProperty]
        string _curveTotalAreaText;

        [ObservableProperty]
        string _curveLoopAreaText;

        [ObservableProperty]
        string _curveInflectionPointsText;

        [ObservableProperty]
        string _curveAsymptotesText;

        // XAML cannot parse float literals for custom controls, so the limits are exposed for bindings
        public float MinimumCurveScale => CurveRelatedConstants.MINIMUM_SCALE_CENTIMETERS;
        public float MaximumCurveScale => CurveRelatedConstants.MAXIMUM_SCALE_CENTIMETERS;
        public float MinimumCurvePointsCount => CurveRelatedConstants.MINIMUM_POINTS_COUNT;
        public float MaximumCurvePointsCount => CurveRelatedConstants.MAXIMUM_POINTS_COUNT;
        public float MinimumCurveTangentLength => CurveRelatedConstants.MINIMUM_TANGENT_LENGTH_CENTIMETERS;

        async partial void OnCurveScaleChanged(float value)
        {
            if (!BernoulliLemniscate.IsValidScale(value) || value < CurveRelatedConstants.MINIMUM_SCALE_CENTIMETERS)
            {
                SetCurveScaleSilent(CurveRelatedConstants.MINIMUM_SCALE_CENTIMETERS);
            }
            else if (value > CurveRelatedConstants.MAXIMUM_SCALE_CENTIMETERS)
            {
                SetCurveScaleSilent(CurveRelatedConstants.MAXIMUM_SCALE_CENTIMETERS);
            }

            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurvePointsCountChanged(float value)
        {
            var pointsCount = float.IsFinite(value)
                ? MathF.Round(value).Clamp(CurveRelatedConstants.MINIMUM_POINTS_COUNT, CurveRelatedConstants.MAXIMUM_POINTS_COUNT)
                : CurveRelatedConstants.DEFAULT_POINTS_COUNT;

            if (pointsCount != value)
            {
                SetCurvePointsCountSilent(pointsCount);
            }

            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurvePointParameterDegreesChanged(float value)
        {
            if (!float.IsFinite(value))
            {
                SetCurvePointParameterDegreesSilent(CurveRelatedConstants.DEFAULT_POINT_PARAMETER_DEGREES);
            }

            await RefreshCurveAsync();
        }

        async partial void OnCurveTangentLengthChanged(float value)
        {
            if (!float.IsFinite(value) || value < CurveRelatedConstants.MINIMUM_TANGENT_LENGTH_CENTIMETERS)
            {
                SetCurveTangentLengthSilent(CurveRelatedConstants.MINIMUM_TANGENT_LENGTH_CENTIMETERS);
            }

            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurveTangentVisibleChanged(bool value)
        {
            if (!value && PickCurvePointByMouse)
            {
                PickCurvePointByMouse = false;
            }

            await RefreshCurveAsync();
        }

        async partial void OnCurveInflectionPointsVisibleChanged(bool _)
        {
            await RefreshCurveAsync();
        }

        async partial void OnCurveRotationPointVisibleChanged(bool _)
        {
            await RefreshCurveAsync();
        }

        partial void OnPickCurvePointByMouseChanged(bool value)
        {
            _curveDraggingTarget = CurveDraggingTarget.None;

            if (value && !CurveTangentVisible)
            {
                CurveTangentVisible = true;
            }

            if (!value)
            {
                ChangeMouseCursor(MouseCursor.Default);
            }
        }

        async partial void OnCurveTranslationXChanged(float value)
        {
            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurveTranslationYChanged(float value)
        {
            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurveRotationPointXChanged(float value)
        {
            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurveRotationPointYChanged(float value)
        {
            await RefreshCurveWithDelayAsync();
        }

        async partial void OnCurveRotationAngleChanged(float value)
        {
            if (!float.IsFinite(value))
            {
                SetCurveRotationAngleSilent(0);
            }

            if (_isCurveInitialized)
            {
                ApplyCurveRotationDelta();
            }

            await RefreshCurveAsync();
        }

        void SetCurveScaleSilent(float newValue)
        {
            _curveScale = newValue;
            OnPropertyChanged(nameof(CurveScale));
        }

        void SetCurvePointsCountSilent(float newValue)
        {
            _curvePointsCount = newValue;
            OnPropertyChanged(nameof(CurvePointsCount));
        }

        void SetCurvePointParameterDegreesSilent(float newValue)
        {
            _curvePointParameterDegrees = newValue;
            OnPropertyChanged(nameof(CurvePointParameterDegrees));
        }

        void SetCurveTangentLengthSilent(float newValue)
        {
            _curveTangentLength = newValue;
            OnPropertyChanged(nameof(CurveTangentLength));
        }

        void SetPickCurvePointByMouseSilent(bool newValue)
        {
            _pickCurvePointByMouse = newValue;
            OnPropertyChanged(nameof(PickCurvePointByMouse));
        }

        void SetCurveTranslationXSilent(float newValue)
        {
            _curveTranslationX = newValue;
            OnPropertyChanged(nameof(CurveTranslationX));
        }

        void SetCurveTranslationYSilent(float newValue)
        {
            _curveTranslationY = newValue;
            OnPropertyChanged(nameof(CurveTranslationY));
        }

        void SetCurveRotationPointXSilent(float newValue)
        {
            _curveRotationPointX = newValue;
            OnPropertyChanged(nameof(CurveRotationPointX));
        }

        void SetCurveRotationPointYSilent(float newValue)
        {
            _curveRotationPointY = newValue;
            OnPropertyChanged(nameof(CurveRotationPointY));
        }

        void SetCurveRotationAngleSilent(float newValue)
        {
            _curveRotationAngle = newValue;
            OnPropertyChanged(nameof(CurveRotationAngle));
        }

        async partial void OnProjectiveStartPointWeightChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveStartPointXChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveStartPointYChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveNewXPointXChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveNewXPointYChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveNewYPointXChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveNewYPointYChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveNewXWeightChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnProjectiveNewYWeightChanged(float _)
        {
            await ApplyProjectiveTransformWithDelayAsync();
        }

        async partial void OnVerticalRotationPointCenterChanged(float value)
        {
            var point = new PointSingle(HorizontalRotationPointCenter, value);
            await ApplyUserPointTranslationTransformWithDelayAsync(point);
        }

        async partial void OnHorizontalRotationPointCenterChanged(float value)
        {
            var point = new PointSingle(value, VerticalRotationPointCenter);
            await ApplyUserPointTranslationTransformWithDelayAsync(point);
        }

        async partial void OnHorizontalFigurePositionChanged(float value)
        {
            var point = new PointSingle(value, VerticalFigurePosition);
            await ApplyFigureTranslationTransformWithDelayAsync(point);
        }

        async partial void OnVerticalFigurePositionChanged(float value)
        {
            var point = new PointSingle(HorizontalFigurePosition, value);
            await ApplyFigureTranslationTransformWithDelayAsync(point);
        }

        async partial void OnAffineStartPointXChanged(float _)
        {
            await ApplyAffineTransformWithDelayAsync();
        }

        async partial void OnAffineStartPointYChanged(float _)
        {
            await ApplyAffineTransformWithDelayAsync();
        }

        async partial void OnAffineNewXPointXChanged(float _)
        {
            await ApplyAffineTransformWithDelayAsync();
        }

        async partial void OnAffineNewXPointYChanged(float _)
        {
            await ApplyAffineTransformWithDelayAsync();
        }

        async partial void OnAffineNewYPointXChanged(float _)
        {
            await ApplyAffineTransformWithDelayAsync();
        }

        async partial void OnAffineNewYPointYChanged(float _)
        {
            await ApplyAffineTransformWithDelayAsync();
        }

        async partial void OnRotationPointVisibleChanged(bool value)
        {
            if (value)
            {
                _wasPositionEditingControlVisible = PositionEditingControlVisible;

                if (PositionEditingControlVisible)
                {
                    _positionEditingControlVisible = false;
                    OnPropertyChanged(nameof(PositionEditingControlVisible));
                }

                _drawingAction = UserPointDrawingAction.AxisPointSelection;
            }
            else
            {
                PositionEditingControlVisible = _wasPositionEditingControlVisible;
            }

            _canRedrawUserPoint = value;

            await RedrawAllAsync();
        }

        partial void OnChangeFigurePositionChanged(bool value)
        {
            if (value)
            {
                PickShapeForResizing = false;
                SizeEditingPanelVisible = false;
                PositionEditingPanelVisible = true;
                PickButtonsVisible = false;
            }

            CancelButtonVisible = value;
        }

        partial void OnNearestSegmentChanged(IPointGeometry oldValue, IPointGeometry newValue)
        {
            if (oldValue is not null)
            {
                oldValue.PointGeometryPropertyChanged -= OnNearestSegmentDimensionChanged;
            }

            if (newValue is not null)
            {
                newValue.PointGeometryPropertyChanged -= OnNearestSegmentDimensionChanged;
                newValue.PointGeometryPropertyChanged += OnNearestSegmentDimensionChanged;

                SizeEditingPanelVisible = true;
                PickButtonsVisible = false;

                PositionEditingPanelVisible = false;
                PositionEditingControlVisible = false;

                CancelButtonVisible = true;

                HandleDistanceDisplaying(newValue);
            }
        }

        partial void OnDimensionLengthCentimetersChanged(float value)
        {
            if (NearestSegment is not null)
            {
                HandleDimensionLengthCentimetersChanged(value);
            }
        }

        async partial void OnPickShapeForResizingChanged(bool value)
        {
            if (!value
                && (_nearestSegment is not null || _pointerMoveNearestSegment is not null)
                && _figure.Any())
            {
                await RedrawAllAsync();

                _pointerMoveNearestSegment = default;
                _drawingAction = UserPointDrawingAction.None;
            }

            if (value)
            {
                ChangeFigurePosition = false;
                _drawingAction = UserPointDrawingAction.FigurePointSelection;
            }
        }

        async partial void OnPositionEditingControlVisibleChanged(bool value)
        {
            if (value)
            {
                _wasRotationPointVisible = RotationPointVisible;

                if (RotationPointVisible)
                {
                    _canRedrawUserPoint = false;
                    _rotationPointVisible = false;
                    OnPropertyChanged(nameof(RotationPointVisible));
                }

                _drawingAction = UserPointDrawingAction.EntireFigureSelection;
            }
            else
            {
                _drawingAction = UserPointDrawingAction.None;
                RotationPointVisible = _wasRotationPointVisible;
            }

            await RedrawAllAsync();
        }

        async partial void OnRotationAngleChanged(float newValue)
        {
            await RotateFigureAsync(newValue);

            UpdateFigureCenterInUserInterface();
        }

        void SetRotationAngleSilent(float newValue)
        {
            _rotationAngle = newValue;
            OnPropertyChanged(nameof(RotationAngle));
        }

        async partial void OnAnimateRotationChanged(bool newValue)
        {
            if (newValue)
            {
                await StartAnimatingRotationAsync();
            }
            else
            {
                await StopAnimatingRotationAsync();
            }
        }

        partial void OnNearestSegmentChanged(IPointGeometry value)
        {
            if (value is not null)
            {
                PickShapeForResizing = false;
                SizeEditingPanelVisible = true;
                PositionEditingPanelVisible = false;
                CancelButtonVisible = true;
            }
        }

        void SetRotationPointControlVisibleSilent(bool value)
        {
            _rotationPointVisible = value;
            OnPropertyChanged(nameof(RotationPointVisible));
        }

        void SetHorizontalFigurePositionSilent(float value)
        {
            _horizontalFigurePosition = value;
            OnPropertyChanged(nameof(HorizontalFigurePosition));
        }

        void SetVerticalFigurePositionSilent(float value)
        {
            _verticalFigurePosition = value;
            OnPropertyChanged(nameof(VerticalFigurePosition));
        }

        void SetHorizontalRotationPointCenterSilent(float value)
        {
            _horizontalRotationPointCenter = value;
            OnPropertyChanged(nameof(HorizontalRotationPointCenter));
        }

        void SetVerticalRotationPointCenterSilent(float value)
        {
            _verticalRotationPointCenter = value;
            OnPropertyChanged(nameof(VerticalRotationPointCenter));
        }

        void SetAffineStartPointXSilent(float newValue)
        {
            _affineStartPointX = newValue;
            OnPropertyChanged(nameof(AffineStartPointX));
        }

        void SetAffineStartPointYSilent(float newValue)
        {
            _affineStartPointY = newValue;
            OnPropertyChanged(nameof(AffineStartPointY));
        }

        void SetAffineNewXPointXSilent(float newValue)
        {
            _affineNewXPointX = newValue;
            OnPropertyChanged(nameof(AffineNewXPointX));
        }

        void SetAffineNewXPointYSilent(float newValue)
        {
            _affineNewXPointY = newValue;
            OnPropertyChanged(nameof(AffineNewXPointY));
        }

        void SetAffineNewYPointXSilent(float newValue)
        {
            _affineNewYPointX = newValue;
            OnPropertyChanged(nameof(AffineNewYPointX));
        }

        void SetAffineNewYPointYSilent(float newValue)
        {
            _affineNewYPointY = newValue;
            OnPropertyChanged(nameof(AffineNewYPointY));
        }

        void SetProjectiveStartPointXSilent(float newValue)
        {
            _projectiveStartPointX = newValue;
            OnPropertyChanged(nameof(ProjectiveStartPointX));
        }

        void SetProjectiveStartPointYSilent(float newValue)
        {
            _projectiveStartPointY = newValue;
            OnPropertyChanged(nameof(ProjectiveStartPointY));
        }

        void SetProjectiveNewXPointXSilent(float newValue)
        {
            _projectiveNewXPointX = newValue;
            OnPropertyChanged(nameof(ProjectiveNewXPointX));
        }

        void SetProjectiveNewXPointYSilent(float newValue)
        {
            _projectiveNewXPointY = newValue;
            OnPropertyChanged(nameof(ProjectiveNewXPointY));
        }

        void SetProjectiveNewYPointXSilent(float newValue)
        {
            _projectiveNewYPointX = newValue;
            OnPropertyChanged(nameof(ProjectiveNewYPointX));
        }

        void SetProjectiveNewYPointYSilent(float newValue)
        {
            _projectiveNewYPointY = newValue;
            OnPropertyChanged(nameof(ProjectiveNewYPointY));
        }

        void SetProjectiveNewXWeightSilent(float newValue)
        {
            _projectiveNewXWeight = newValue;
            OnPropertyChanged(nameof(ProjectiveNewXWeight));
        }

        void SetProjectiveNewYWeightSilent(float newValue)
        {
            _projectiveNewYWeight = newValue;
            OnPropertyChanged(nameof(ProjectiveNewYWeight));
        }

        void SetProjectiveStartPointWeightSilent(float newValue)
        {
            _projectiveStartPointWeight = newValue;
            OnPropertyChanged(nameof(ProjectiveStartPointWeight));
        }

        [ObservableProperty]
        bool _contourPolygonVisible = true;

        [ObservableProperty]
        float _contourSegmentPointsCount;

        [ObservableProperty]
        float _contourControlWeight;

        [ObservableProperty]
        float _selectedContourPointWeight;

        [ObservableProperty]
        bool _isContourAnimationRunning;

        [ObservableProperty]
        float _contourTranslationX;

        [ObservableProperty]
        float _contourTranslationY;

        [ObservableProperty]
        float _contourRotationPointX;

        [ObservableProperty]
        float _contourRotationPointY;

        [ObservableProperty]
        float _contourRotationAngle;

        [ObservableProperty]
        bool _contourRotationPointVisible = true;

        [ObservableProperty]
        string _contourInfoText;

        [ObservableProperty]
        string _contourSelectedPointText;

        [ObservableProperty]
        string _contourSelectedSegmentText;

        [ObservableProperty]
        string _contourMorphProgressText;

        public float MinimumContourSegmentPointsCount => ContourRelatedConstants.MINIMUM_SEGMENT_POINTS_COUNT;
        public float MaximumContourSegmentPointsCount => ContourRelatedConstants.MAXIMUM_SEGMENT_POINTS_COUNT;
        public float MinimumContourWeight => ContourRelatedConstants.MINIMUM_WEIGHT;
        public float MaximumContourWeight => ContourRelatedConstants.MAXIMUM_WEIGHT;

        static float ValidateContourWeight(float value)
            => float.IsFinite(value)
                ? value.Clamp(ContourRelatedConstants.MINIMUM_WEIGHT, ContourRelatedConstants.MAXIMUM_WEIGHT)
                : ContourRelatedConstants.DEFAULT_WEIGHT;

        async partial void OnContourPolygonVisibleChanged(bool _)
        {
            await RefreshContourAsync();
        }

        async partial void OnContourSegmentPointsCountChanged(float value)
        {
            var pointsCount = float.IsFinite(value)
                ? MathF.Round(value).Clamp(ContourRelatedConstants.MINIMUM_SEGMENT_POINTS_COUNT, ContourRelatedConstants.MAXIMUM_SEGMENT_POINTS_COUNT)
                : ContourRelatedConstants.DEFAULT_SEGMENT_POINTS_COUNT;

            if (pointsCount != value)
            {
                SetContourSegmentPointsCountSilent(pointsCount);
            }

            await RefreshContourWithDelayAsync();
        }

        async partial void OnContourControlWeightChanged(float value)
        {
            var weight = ValidateContourWeight(value);

            if (weight != value)
            {
                SetContourControlWeightSilent(weight);
            }

            ApplyContourControlWeight(weight);

            await RefreshContourWithDelayAsync();
        }

        async partial void OnSelectedContourPointWeightChanged(float value)
        {
            var weight = ValidateContourWeight(value);

            if (weight != value)
            {
                SetSelectedContourPointWeightSilent(weight);
            }

            ApplySelectedContourPointWeight(weight);

            await RefreshContourWithDelayAsync();
        }

        async partial void OnContourTranslationXChanged(float value)
        {
            await RefreshContourWithDelayAsync();
        }

        async partial void OnContourTranslationYChanged(float value)
        {
            await RefreshContourWithDelayAsync();
        }

        async partial void OnContourRotationPointXChanged(float value)
        {
            await RefreshContourWithDelayAsync();
        }

        async partial void OnContourRotationPointYChanged(float value)
        {
            await RefreshContourWithDelayAsync();
        }

        async partial void OnContourRotationAngleChanged(float value)
        {
            if (!float.IsFinite(value))
            {
                SetContourRotationAngleSilent(0);
            }

            if (_isContourInitialized)
            {
                ApplyContourRotationDelta();
            }

            await RefreshContourAsync();
        }

        async partial void OnContourRotationPointVisibleChanged(bool _)
        {
            await RefreshContourAsync();
        }

        void SetContourSegmentPointsCountSilent(float newValue)
        {
            _contourSegmentPointsCount = newValue;
            OnPropertyChanged(nameof(ContourSegmentPointsCount));
        }

        void SetContourControlWeightSilent(float newValue)
        {
            _contourControlWeight = newValue;
            OnPropertyChanged(nameof(ContourControlWeight));
        }

        void SetSelectedContourPointWeightSilent(float newValue)
        {
            _selectedContourPointWeight = newValue;
            OnPropertyChanged(nameof(SelectedContourPointWeight));
        }

        void SetContourTranslationXSilent(float newValue)
        {
            _contourTranslationX = newValue;
            OnPropertyChanged(nameof(ContourTranslationX));
        }

        void SetContourTranslationYSilent(float newValue)
        {
            _contourTranslationY = newValue;
            OnPropertyChanged(nameof(ContourTranslationY));
        }

        void SetContourRotationPointXSilent(float newValue)
        {
            _contourRotationPointX = newValue;
            OnPropertyChanged(nameof(ContourRotationPointX));
        }

        void SetContourRotationPointYSilent(float newValue)
        {
            _contourRotationPointY = newValue;
            OnPropertyChanged(nameof(ContourRotationPointY));
        }

        void SetContourRotationAngleSilent(float newValue)
        {
            _contourRotationAngle = newValue;
            OnPropertyChanged(nameof(ContourRotationAngle));
        }

        [ObservableProperty]
        bool _isSmoothContourMode;

        [ObservableProperty]
        bool _isSelectedContourPointNode;

        [ObservableProperty]
        bool _selectedContourPointIsBreakPoint;

        [ObservableProperty]
        string _contourSmoothnessText;

        partial void OnIsSmoothContourModeChanged(bool value)
        {
            OnPropertyChanged(nameof(ContourTitleText));
            OnPropertyChanged(nameof(ContourPolygonHintText));
        }

        public string ContourTitleText => IsSmoothContourMode
            ? "Smooth curvilinear contour: poodle"
            : "Curvilinear contour: poodle";

        public string ContourPolygonHintText => IsSmoothContourMode
            ? "Drag the points with the mouse: green points are reference points, blue points are smooth nodes, red points are break points. Double click a node to switch it between a smooth node and a break point."
            : "Drag the reference points with the mouse: blue points are arc ends A, orange points are control points B. The selected point is red.";

        async partial void OnSelectedContourPointIsBreakPointChanged(bool value)
        {
            if (_isContourInitialized && HasSelectedContourPoint && IsSelectedContourPointNode)
            {
                await SetContourBreakPointAsync(_selectedContourIndex, _selectedContourPointIndex, value);
            }
        }

        void SetSelectedContourPointNodeStateSilent(bool isNode, bool isBreakPoint)
        {
            IsSelectedContourPointNode = isNode;

            _selectedContourPointIsBreakPoint = isBreakPoint;
            OnPropertyChanged(nameof(SelectedContourPointIsBreakPoint));
        }

        [ObservableProperty]
        float _polyhedronSidesCount;

        [ObservableProperty]
        float _polyhedronRadius;

        [ObservableProperty]
        float _polyhedronHeight;

        [ObservableProperty]
        float _polyhedronRoofHeight;

        [ObservableProperty]
        float _polyhedronScale;

        [ObservableProperty]
        float _polyhedronTranslationX;

        [ObservableProperty]
        float _polyhedronTranslationY;

        [ObservableProperty]
        float _polyhedronTranslationZ;

        [ObservableProperty]
        float _polyhedronRotationX;

        [ObservableProperty]
        float _polyhedronRotationY;

        [ObservableProperty]
        float _polyhedronRotationZ;

        [ObservableProperty]
        float _projectionCenterX;

        [ObservableProperty]
        float _projectionCenterY;

        [ObservableProperty]
        float _projectionCenterZ;

        [ObservableProperty]
        float _polyhedronAnimationHeightMinimum;

        [ObservableProperty]
        float _polyhedronAnimationHeightMaximum;

        [ObservableProperty]
        float _polyhedronAnimationRotationStepX;

        [ObservableProperty]
        float _polyhedronAnimationRotationStepY;

        [ObservableProperty]
        float _polyhedronAnimationRotationStepZ;

        [ObservableProperty]
        string _polyhedronInfoText;

        [ObservableProperty]
        string _polyhedronPivotText;

        [ObservableProperty]
        string _projectionMatrixText;

        [ObservableProperty]
        string _vanishingPointsText;

        [ObservableProperty]
        bool _isPolyhedronAnimationRunning;

        [ObservableProperty]
        bool _polyhedronOwnAxesVisible = true;

        [ObservableProperty]
        bool _vanishingPointsVisible = true;

        public float MinimumPolyhedronSidesCount => PolyhedronRelatedConstants.MINIMUM_SIDES_COUNT;
        public float MaximumPolyhedronSidesCount => PolyhedronRelatedConstants.MAXIMUM_SIDES_COUNT;
        public float MinimumPolyhedronSize => PolyhedronRelatedConstants.MINIMUM_SIZE_CENTIMETERS;
        public float MaximumPolyhedronSize => PolyhedronRelatedConstants.MAXIMUM_SIZE_CENTIMETERS;
        public float MinimumPolyhedronScale => PolyhedronRelatedConstants.MINIMUM_SCALE;
        public float MaximumPolyhedronScale => PolyhedronRelatedConstants.MAXIMUM_SCALE;
        public float MinimumPolyhedronTranslation => -PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS;
        public float MaximumPolyhedronTranslation => PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS;
        public float MinimumProjectionCenter => -PolyhedronRelatedConstants.MAXIMUM_CENTER_CENTIMETERS;
        public float MaximumProjectionCenter => PolyhedronRelatedConstants.MAXIMUM_CENTER_CENTIMETERS;
        public float MinimumAnimationRotationStep => -PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES;
        public float MaximumAnimationRotationStep => PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES;

        // a center of projection must not lie on the plane x = 0: |s| >= 1 cm, the sign chooses the side
        static float ClampCenterDistance(float value)
        {
            var distance = MathF.Abs(value).Clamp(PolyhedronRelatedConstants.MINIMUM_CENTER_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_CENTER_CENTIMETERS);

            return value < 0 ? -distance : distance;
        }

        async partial void OnPolyhedronOwnAxesVisibleChanged(bool _)
        {
            await RefreshProjectionSceneAsync();
        }

        async partial void OnVanishingPointsVisibleChanged(bool _)
        {
            await RefreshProjectionSceneAsync();
        }

        async partial void OnPolyhedronSidesCountChanged(float value)
        {
            var validValue = float.IsFinite(value) ? MathF.Round(value).Clamp(PolyhedronRelatedConstants.MINIMUM_SIDES_COUNT, PolyhedronRelatedConstants.MAXIMUM_SIDES_COUNT) : PolyhedronRelatedConstants.DEFAULT_SIDES_COUNT;

            if (validValue != value)
            {
                SetPolyhedronSidesCountSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronSidesCountSilent(float newValue)
        {
            _polyhedronSidesCount = newValue;
            OnPropertyChanged(nameof(PolyhedronSidesCount));
        }

        async partial void OnPolyhedronRadiusChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(PolyhedronRelatedConstants.MINIMUM_SIZE_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_SIZE_CENTIMETERS) : PolyhedronRelatedConstants.DEFAULT_HEIGHT_CENTIMETERS;

            if (validValue != value)
            {
                SetPolyhedronRadiusSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronRadiusSilent(float newValue)
        {
            _polyhedronRadius = newValue;
            OnPropertyChanged(nameof(PolyhedronRadius));
        }

        async partial void OnPolyhedronHeightChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(PolyhedronRelatedConstants.MINIMUM_SIZE_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_SIZE_CENTIMETERS) : PolyhedronRelatedConstants.DEFAULT_HEIGHT_CENTIMETERS;

            if (validValue != value)
            {
                SetPolyhedronHeightSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronHeightSilent(float newValue)
        {
            _polyhedronHeight = newValue;
            OnPropertyChanged(nameof(PolyhedronHeight));
        }

        async partial void OnPolyhedronRoofHeightChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(0, PolyhedronRelatedConstants.MAXIMUM_SIZE_CENTIMETERS) : PolyhedronRelatedConstants.DEFAULT_ROOF_HEIGHT_CENTIMETERS;

            if (validValue != value)
            {
                SetPolyhedronRoofHeightSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronRoofHeightSilent(float newValue)
        {
            _polyhedronRoofHeight = newValue;
            OnPropertyChanged(nameof(PolyhedronRoofHeight));
        }

        async partial void OnPolyhedronScaleChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(PolyhedronRelatedConstants.MINIMUM_SCALE, PolyhedronRelatedConstants.MAXIMUM_SCALE) : PolyhedronRelatedConstants.DEFAULT_SCALE;

            if (validValue != value)
            {
                SetPolyhedronScaleSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronScaleSilent(float newValue)
        {
            _polyhedronScale = newValue;
            OnPropertyChanged(nameof(PolyhedronScale));
        }

        async partial void OnPolyhedronTranslationXChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS) : 0;

            if (validValue != value)
            {
                SetPolyhedronTranslationXSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronTranslationXSilent(float newValue)
        {
            _polyhedronTranslationX = newValue;
            OnPropertyChanged(nameof(PolyhedronTranslationX));
        }

        async partial void OnPolyhedronTranslationYChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS) : 0;

            if (validValue != value)
            {
                SetPolyhedronTranslationYSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronTranslationYSilent(float newValue)
        {
            _polyhedronTranslationY = newValue;
            OnPropertyChanged(nameof(PolyhedronTranslationY));
        }

        async partial void OnPolyhedronTranslationZChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS) : 0;

            if (validValue != value)
            {
                SetPolyhedronTranslationZSilent(validValue);
            }

            await RefreshPolyhedronWithDelayAsync();
        }

        void SetPolyhedronTranslationZSilent(float newValue)
        {
            _polyhedronTranslationZ = newValue;
            OnPropertyChanged(nameof(PolyhedronTranslationZ));
        }

        async partial void OnPolyhedronRotationXChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetPolyhedronRotationXSilent(validValue);
            }

            await RefreshPolyhedronAsync();
        }

        void SetPolyhedronRotationXSilent(float newValue)
        {
            _polyhedronRotationX = newValue;
            OnPropertyChanged(nameof(PolyhedronRotationX));
        }

        async partial void OnPolyhedronRotationYChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetPolyhedronRotationYSilent(validValue);
            }

            await RefreshPolyhedronAsync();
        }

        void SetPolyhedronRotationYSilent(float newValue)
        {
            _polyhedronRotationY = newValue;
            OnPropertyChanged(nameof(PolyhedronRotationY));
        }

        async partial void OnPolyhedronRotationZChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetPolyhedronRotationZSilent(validValue);
            }

            await RefreshPolyhedronAsync();
        }

        void SetPolyhedronRotationZSilent(float newValue)
        {
            _polyhedronRotationZ = newValue;
            OnPropertyChanged(nameof(PolyhedronRotationZ));
        }

        async partial void OnProjectionCenterXChanged(float value)
        {
            var validValue = float.IsFinite(value) ? ClampCenterDistance(value) : PolyhedronRelatedConstants.DEFAULT_CENTER_X_CENTIMETERS;

            if (validValue != value)
            {
                SetProjectionCenterXSilent(validValue);
            }

            await RefreshProjectionSceneWithDelayAsync();
        }

        void SetProjectionCenterXSilent(float newValue)
        {
            _projectionCenterX = newValue;
            OnPropertyChanged(nameof(ProjectionCenterX));
        }

        async partial void OnProjectionCenterYChanged(float value)
        {
            var validValue = float.IsFinite(value) ? ClampCenterDistance(value) : PolyhedronRelatedConstants.DEFAULT_CENTER_X_CENTIMETERS;

            if (validValue != value)
            {
                SetProjectionCenterYSilent(validValue);
            }

            await RefreshProjectionSceneWithDelayAsync();
        }

        void SetProjectionCenterYSilent(float newValue)
        {
            _projectionCenterY = newValue;
            OnPropertyChanged(nameof(ProjectionCenterY));
        }

        async partial void OnProjectionCenterZChanged(float value)
        {
            var validValue = float.IsFinite(value) ? ClampCenterDistance(value) : PolyhedronRelatedConstants.DEFAULT_CENTER_X_CENTIMETERS;

            if (validValue != value)
            {
                SetProjectionCenterZSilent(validValue);
            }

            await RefreshProjectionSceneWithDelayAsync();
        }

        void SetProjectionCenterZSilent(float newValue)
        {
            _projectionCenterZ = newValue;
            OnPropertyChanged(nameof(ProjectionCenterZ));
        }

        partial void OnPolyhedronAnimationHeightMinimumChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(PolyhedronRelatedConstants.MINIMUM_SIZE_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_SIZE_CENTIMETERS) : PolyhedronRelatedConstants.DEFAULT_HEIGHT_CENTIMETERS;

            if (validValue != value)
            {
                SetPolyhedronAnimationHeightMinimumSilent(validValue);
            }

        }

        void SetPolyhedronAnimationHeightMinimumSilent(float newValue)
        {
            _polyhedronAnimationHeightMinimum = newValue;
            OnPropertyChanged(nameof(PolyhedronAnimationHeightMinimum));
        }

        partial void OnPolyhedronAnimationHeightMaximumChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(PolyhedronRelatedConstants.MINIMUM_SIZE_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_SIZE_CENTIMETERS) : PolyhedronRelatedConstants.DEFAULT_HEIGHT_CENTIMETERS;

            if (validValue != value)
            {
                SetPolyhedronAnimationHeightMaximumSilent(validValue);
            }

        }

        void SetPolyhedronAnimationHeightMaximumSilent(float newValue)
        {
            _polyhedronAnimationHeightMaximum = newValue;
            OnPropertyChanged(nameof(PolyhedronAnimationHeightMaximum));
        }

        partial void OnPolyhedronAnimationRotationStepXChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES, PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES) : 0;

            if (validValue != value)
            {
                SetPolyhedronAnimationRotationStepXSilent(validValue);
            }

        }

        void SetPolyhedronAnimationRotationStepXSilent(float newValue)
        {
            _polyhedronAnimationRotationStepX = newValue;
            OnPropertyChanged(nameof(PolyhedronAnimationRotationStepX));
        }

        partial void OnPolyhedronAnimationRotationStepYChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES, PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES) : 0;

            if (validValue != value)
            {
                SetPolyhedronAnimationRotationStepYSilent(validValue);
            }

        }

        void SetPolyhedronAnimationRotationStepYSilent(float newValue)
        {
            _polyhedronAnimationRotationStepY = newValue;
            OnPropertyChanged(nameof(PolyhedronAnimationRotationStepY));
        }

        partial void OnPolyhedronAnimationRotationStepZChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES, PolyhedronRelatedConstants.MAXIMUM_ANIMATION_ROTATION_STEP_DEGREES) : 0;

            if (validValue != value)
            {
                SetPolyhedronAnimationRotationStepZSilent(validValue);
            }

        }

        void SetPolyhedronAnimationRotationStepZSilent(float newValue)
        {
            _polyhedronAnimationRotationStepZ = newValue;
            OnPropertyChanged(nameof(PolyhedronAnimationRotationStepZ));
        }

        // Lab 6: surface, contour on the surface

        [ObservableProperty]
        float _surfaceRadius;

        [ObservableProperty]
        float _surfaceDepthCoefficient;

        [ObservableProperty]
        float _surfaceMeshStepU;

        [ObservableProperty]
        float _surfaceMeshStepV;

        [ObservableProperty]
        float _surfaceTranslationX;

        [ObservableProperty]
        float _surfaceTranslationY;

        [ObservableProperty]
        float _surfaceTranslationZ;

        [ObservableProperty]
        float _surfaceRotationX;

        [ObservableProperty]
        float _surfaceRotationY;

        [ObservableProperty]
        float _surfaceRotationZ;

        [ObservableProperty]
        float _surfaceContourShiftU;

        [ObservableProperty]
        float _surfaceContourShiftV;

        [ObservableProperty]
        float _surfaceContourAngle;

        [ObservableProperty]
        float _surfaceContourScale;

        [ObservableProperty]
        float _surfaceAnimationRadiusMinimum;

        [ObservableProperty]
        float _surfaceAnimationRadiusMaximum;

        [ObservableProperty]
        float _surfaceAnimationDepthMinimum;

        [ObservableProperty]
        float _surfaceAnimationDepthMaximum;

        [ObservableProperty]
        string _surfaceInfoText;

        [ObservableProperty]
        string _surfacePivotText;

        [ObservableProperty]
        string _surfaceContourText;

        [ObservableProperty]
        bool _isSurfaceAnimationRunning;

        [ObservableProperty]
        bool _surfaceContourVisible = true;

        public float MinimumSurfaceRadius => SurfaceRelatedConstants.MINIMUM_RADIUS_CENTIMETERS;
        public float MaximumSurfaceRadius => SurfaceRelatedConstants.MAXIMUM_RADIUS_CENTIMETERS;
        public float MinimumSurfaceDepth => SurfaceRelatedConstants.MINIMUM_DEPTH_COEFFICIENT;
        public float MaximumSurfaceDepth => SurfaceRelatedConstants.MAXIMUM_DEPTH_COEFFICIENT;
        public float MinimumSurfaceMeshStep => SurfaceRelatedConstants.MINIMUM_MESH_STEP_DEGREES;
        public float MaximumSurfaceMeshStep => SurfaceRelatedConstants.MAXIMUM_MESH_STEP_DEGREES;
        public float MinimumSurfaceContourShift => -SurfaceRelatedConstants.MAXIMUM_CONTOUR_SHIFT_DEGREES;
        public float MaximumSurfaceContourShift => SurfaceRelatedConstants.MAXIMUM_CONTOUR_SHIFT_DEGREES;
        public float MinimumSurfaceContourScale => SurfaceRelatedConstants.MINIMUM_CONTOUR_SCALE;
        public float MaximumSurfaceContourScale => SurfaceRelatedConstants.MAXIMUM_CONTOUR_SCALE;

        async partial void OnSurfaceContourVisibleChanged(bool _)
        {
            await RefreshSurfaceAsync();
        }

        async partial void OnSurfaceRadiusChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_RADIUS_CENTIMETERS, SurfaceRelatedConstants.MAXIMUM_RADIUS_CENTIMETERS) : SurfaceRelatedConstants.DEFAULT_RADIUS_CENTIMETERS;

            if (validValue != value)
            {
                SetSurfaceRadiusSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceRadiusSilent(float newValue)
        {
            _surfaceRadius = newValue;
            OnPropertyChanged(nameof(SurfaceRadius));
        }

        async partial void OnSurfaceDepthCoefficientChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_DEPTH_COEFFICIENT, SurfaceRelatedConstants.MAXIMUM_DEPTH_COEFFICIENT) : SurfaceRelatedConstants.DEFAULT_DEPTH_COEFFICIENT;

            if (validValue != value)
            {
                SetSurfaceDepthCoefficientSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceDepthCoefficientSilent(float newValue)
        {
            _surfaceDepthCoefficient = newValue;
            OnPropertyChanged(nameof(SurfaceDepthCoefficient));
        }

        async partial void OnSurfaceMeshStepUChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_MESH_STEP_DEGREES, SurfaceRelatedConstants.MAXIMUM_MESH_STEP_DEGREES) : SurfaceRelatedConstants.DEFAULT_MESH_STEP_DEGREES;

            if (validValue != value)
            {
                SetSurfaceMeshStepUSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceMeshStepUSilent(float newValue)
        {
            _surfaceMeshStepU = newValue;
            OnPropertyChanged(nameof(SurfaceMeshStepU));
        }

        async partial void OnSurfaceMeshStepVChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_MESH_STEP_DEGREES, SurfaceRelatedConstants.MAXIMUM_MESH_STEP_DEGREES) : SurfaceRelatedConstants.DEFAULT_MESH_STEP_DEGREES;

            if (validValue != value)
            {
                SetSurfaceMeshStepVSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceMeshStepVSilent(float newValue)
        {
            _surfaceMeshStepV = newValue;
            OnPropertyChanged(nameof(SurfaceMeshStepV));
        }

        async partial void OnSurfaceTranslationXChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS) : 0;

            if (validValue != value)
            {
                SetSurfaceTranslationXSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceTranslationXSilent(float newValue)
        {
            _surfaceTranslationX = newValue;
            OnPropertyChanged(nameof(SurfaceTranslationX));
        }

        async partial void OnSurfaceTranslationYChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS) : 0;

            if (validValue != value)
            {
                SetSurfaceTranslationYSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceTranslationYSilent(float newValue)
        {
            _surfaceTranslationY = newValue;
            OnPropertyChanged(nameof(SurfaceTranslationY));
        }

        async partial void OnSurfaceTranslationZChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS, PolyhedronRelatedConstants.MAXIMUM_TRANSLATION_CENTIMETERS) : 0;

            if (validValue != value)
            {
                SetSurfaceTranslationZSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceTranslationZSilent(float newValue)
        {
            _surfaceTranslationZ = newValue;
            OnPropertyChanged(nameof(SurfaceTranslationZ));
        }

        async partial void OnSurfaceRotationXChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetSurfaceRotationXSilent(validValue);
            }

            await RefreshSurfaceAsync();
        }

        void SetSurfaceRotationXSilent(float newValue)
        {
            _surfaceRotationX = newValue;
            OnPropertyChanged(nameof(SurfaceRotationX));
        }

        async partial void OnSurfaceRotationYChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetSurfaceRotationYSilent(validValue);
            }

            await RefreshSurfaceAsync();
        }

        void SetSurfaceRotationYSilent(float newValue)
        {
            _surfaceRotationY = newValue;
            OnPropertyChanged(nameof(SurfaceRotationY));
        }

        async partial void OnSurfaceRotationZChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetSurfaceRotationZSilent(validValue);
            }

            await RefreshSurfaceAsync();
        }

        void SetSurfaceRotationZSilent(float newValue)
        {
            _surfaceRotationZ = newValue;
            OnPropertyChanged(nameof(SurfaceRotationZ));
        }

        async partial void OnSurfaceContourShiftUChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-SurfaceRelatedConstants.MAXIMUM_CONTOUR_SHIFT_DEGREES, SurfaceRelatedConstants.MAXIMUM_CONTOUR_SHIFT_DEGREES) : 0;

            if (validValue != value)
            {
                SetSurfaceContourShiftUSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceContourShiftUSilent(float newValue)
        {
            _surfaceContourShiftU = newValue;
            OnPropertyChanged(nameof(SurfaceContourShiftU));
        }

        async partial void OnSurfaceContourShiftVChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(-SurfaceRelatedConstants.MAXIMUM_CONTOUR_SHIFT_DEGREES, SurfaceRelatedConstants.MAXIMUM_CONTOUR_SHIFT_DEGREES) : 0;

            if (validValue != value)
            {
                SetSurfaceContourShiftVSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceContourShiftVSilent(float newValue)
        {
            _surfaceContourShiftV = newValue;
            OnPropertyChanged(nameof(SurfaceContourShiftV));
        }

        async partial void OnSurfaceContourAngleChanged(float value)
        {
            var validValue = float.IsFinite(value) ? WrapAngle(value) : 0;

            if (validValue != value)
            {
                SetSurfaceContourAngleSilent(validValue);
            }

            await RefreshSurfaceAsync();
        }

        void SetSurfaceContourAngleSilent(float newValue)
        {
            _surfaceContourAngle = newValue;
            OnPropertyChanged(nameof(SurfaceContourAngle));
        }

        async partial void OnSurfaceContourScaleChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_CONTOUR_SCALE, SurfaceRelatedConstants.MAXIMUM_CONTOUR_SCALE) : SurfaceRelatedConstants.DEFAULT_CONTOUR_SCALE_DEGREES_PER_CENTIMETER;

            if (validValue != value)
            {
                SetSurfaceContourScaleSilent(validValue);
            }

            await RefreshSurfaceWithDelayAsync();
        }

        void SetSurfaceContourScaleSilent(float newValue)
        {
            _surfaceContourScale = newValue;
            OnPropertyChanged(nameof(SurfaceContourScale));
        }

        partial void OnSurfaceAnimationRadiusMinimumChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_RADIUS_CENTIMETERS, SurfaceRelatedConstants.MAXIMUM_RADIUS_CENTIMETERS) : SurfaceRelatedConstants.DEFAULT_ANIMATION_RADIUS_MINIMUM;

            if (validValue != value)
            {
                SetSurfaceAnimationRadiusMinimumSilent(validValue);
            }
        }

        void SetSurfaceAnimationRadiusMinimumSilent(float newValue)
        {
            _surfaceAnimationRadiusMinimum = newValue;
            OnPropertyChanged(nameof(SurfaceAnimationRadiusMinimum));
        }

        partial void OnSurfaceAnimationRadiusMaximumChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_RADIUS_CENTIMETERS, SurfaceRelatedConstants.MAXIMUM_RADIUS_CENTIMETERS) : SurfaceRelatedConstants.DEFAULT_ANIMATION_RADIUS_MAXIMUM;

            if (validValue != value)
            {
                SetSurfaceAnimationRadiusMaximumSilent(validValue);
            }
        }

        void SetSurfaceAnimationRadiusMaximumSilent(float newValue)
        {
            _surfaceAnimationRadiusMaximum = newValue;
            OnPropertyChanged(nameof(SurfaceAnimationRadiusMaximum));
        }

        partial void OnSurfaceAnimationDepthMinimumChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_DEPTH_COEFFICIENT, SurfaceRelatedConstants.MAXIMUM_DEPTH_COEFFICIENT) : SurfaceRelatedConstants.DEFAULT_ANIMATION_DEPTH_MINIMUM;

            if (validValue != value)
            {
                SetSurfaceAnimationDepthMinimumSilent(validValue);
            }
        }

        void SetSurfaceAnimationDepthMinimumSilent(float newValue)
        {
            _surfaceAnimationDepthMinimum = newValue;
            OnPropertyChanged(nameof(SurfaceAnimationDepthMinimum));
        }

        partial void OnSurfaceAnimationDepthMaximumChanged(float value)
        {
            var validValue = float.IsFinite(value) ? value.Clamp(SurfaceRelatedConstants.MINIMUM_DEPTH_COEFFICIENT, SurfaceRelatedConstants.MAXIMUM_DEPTH_COEFFICIENT) : SurfaceRelatedConstants.DEFAULT_ANIMATION_DEPTH_MAXIMUM;

            if (validValue != value)
            {
                SetSurfaceAnimationDepthMaximumSilent(validValue);
            }
        }

        void SetSurfaceAnimationDepthMaximumSilent(float newValue)
        {
            _surfaceAnimationDepthMaximum = newValue;
            OnPropertyChanged(nameof(SurfaceAnimationDepthMaximum));
        }
    }
}
