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
    }
}
