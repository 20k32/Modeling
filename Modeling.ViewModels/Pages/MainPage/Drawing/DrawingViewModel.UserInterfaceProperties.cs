using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.Drawing;
using Modeling.Models.Drawing.Figures.PointGeometries;
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
    }
}
