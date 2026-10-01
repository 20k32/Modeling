using CommunityToolkit.Mvvm.ComponentModel;
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
        bool _additionalPanelVisible;

        [ObservableProperty]
        bool _lineEditingPanelVisible;

        [ObservableProperty]
        bool _circleEditingPanelVisible;

        [ObservableProperty]
        IPointGeometry _nearestSegment;

        [ObservableProperty]
        float _dimensionLength;

        partial void OnChangeFigurePositionChanged(bool value)
        {
            if (value)
            {
                PickShapeForResizing = false;
            }
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

                HandleDistanceDisplaying(newValue);
            }
        }

        partial void OnDimensionLengthChanged(float value)
        {
            if (NearestSegment is not null)
            {
                HandleDimensionLengthChanged(value);
            }
        }

        partial void OnPickShapeForResizingChanged(bool value)
        {
            if (!value
                && _nearestSegment is not null
                && _figure.Any())
            {
                RedrawFigure();
                AdditionalPanelVisible = true;
            }

            if (value)
            {
                ChangeFigurePosition = false;
            }
        }
    }
}
