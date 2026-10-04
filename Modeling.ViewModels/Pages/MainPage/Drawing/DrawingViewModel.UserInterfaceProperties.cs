using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;
using System.Linq;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    public sealed partial class DrawingViewModel : ObservableObject
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

        bool _wasRotationPointVisible;
        bool _wasPositionEditingControlVisible;

        partial void OnRotationPointVisibleChanged(bool value)
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

            RedrawAll();
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

        partial void OnPickShapeForResizingChanged(bool value)
        {
            if (!value
                && (_nearestSegment is not null || _pointerMoveNearestSegment is not null)
                && _figure.Any())
            {
                RedrawAll();

                _pointerMoveNearestSegment = default;
                _drawingAction = UserPointDrawingAction.None;
            }

            if (value)
            {
                ChangeFigurePosition = false;
                _drawingAction = UserPointDrawingAction.FigurePointSelection;
            }
        }

        partial void OnPositionEditingControlVisibleChanged(bool value)
        {
            if (value)
            {
                _wasRotationPointVisible = RotationPointVisible;

                if (RotationPointVisible)
                {
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

            RedrawAll();
        }

        partial void OnRotationAngleChanged(float newValue)
        {
            RotateFigure(newValue);
        }

        private void SetRotationAngleSilent(float newValue)
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
    }
}
