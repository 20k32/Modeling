using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Graphics;
using Windows.UI;

namespace Modeling.Core.Settings
{
    sealed class DrawingSettings : IDrawingSettings
    {
        public bool? Initialized { get; set; }

        public event ActionEventHandler SettingsChanged;

        private void InvokeSettingsChanged()
        {
            if (ShouldInvokeSettingsChanged
                && SettingsChanged is not null)
            {
                SettingsChanged.Invoke();
            }
        }

        private int _dpiX;
        public int DpiX
        {
            get => _dpiX;
            set
            {
                if (_dpiX != value)
                {
                    _dpiX = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private int _dpiY;
        public int DpiY
        {
            get => _dpiY;
            set
            {
                if (_dpiY != value)
                {
                    _dpiY = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _drawingColor;
        public Color DrawingColor
        {
            get => _drawingColor;
            set
            {
                if (_drawingColor != value)
                {
                    _drawingColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _gridDrawingThickness;
        public float GridDrawingThickness
        {
            get => _gridDrawingThickness;
            set
            {
                if (_gridDrawingThickness != value)
                {
                    _gridDrawingThickness = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _figureDrawingThickness;
        public float FigureDrawingThickness
        {
            get => _figureDrawingThickness;
            set
            {
                if (_figureDrawingThickness != value)
                {
                    _figureDrawingThickness = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _scale;
        public float Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private PointSingle _centerCanvasPosition;
        public PointSingle CenterCanvasPosition
        {
            get => _centerCanvasPosition;
            set
            {
                if (_centerCanvasPosition != value)
                {
                    _centerCanvasPosition = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private PointSingle _rotatePointPosition;
        public PointSingle RotatePointPosition
        {
            get => _rotatePointPosition;
            set
            {
                if (_rotatePointPosition != value)
                {
                    _rotatePointPosition = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private ICollection<PointSingle> _figure;
        public ICollection<PointSingle> Figure
        {
            get => _figure;
            set
            {
                if (_figure != value)
                {
                    _figure = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _displayMarkInCanvasCenter;
        public bool DisplayMarkInCanvasCenter
        {
            get => _displayMarkInCanvasCenter;
            set
            {
                if (_displayMarkInCanvasCenter != value)
                {
                    _displayMarkInCanvasCenter = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _displayGrid;
        public bool DisplayGrid
        {
            get => _displayGrid;
            set
            {
                if (_displayGrid != value)
                {
                    _displayGrid = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _displayAxis;
        public bool DisplayAxis
        {
            get => _displayAxis;
            set
            {
                if (_displayAxis != value)
                {
                    _displayAxis = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _pixelsPerCentimeter;
        public float PixelsPerCentimeter
        {
            get => _pixelsPerCentimeter;
            set
            {
                if (_pixelsPerCentimeter != value)
                {
                    _pixelsPerCentimeter = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private SizeInt32 _canvasSize;
        public SizeInt32 CanvasSize
        {
            get => _canvasSize;
            set
            {
                if (_canvasSize != value)
                {
                    _canvasSize = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _horizontalAxisColor;
        public Color HorizontalAxisColor
        {
            get => _horizontalAxisColor;
            set
            {
                if (_horizontalAxisColor != value)
                {
                    _horizontalAxisColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _verticalAxisColor;
        public Color VerticalAxisColor
        {
            get => _verticalAxisColor;
            set
            {
                if (_verticalAxisColor != value)
                {
                    _verticalAxisColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private ICollection<PointSingle> _horizontalAxis;
        public ICollection<PointSingle> HorizontalAxis
        {
            get => _horizontalAxis;
            set
            {
                if (_horizontalAxis != value)
                {
                    _horizontalAxis = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private ICollection<PointSingle> _verticalAxis;
        public ICollection<PointSingle> VerticalAxis
        {
            get => _verticalAxis;
            set
            {
                if (_verticalAxis != value)
                {
                    _verticalAxis = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _verticalAxisTicksColor;
        public Color VerticalAxisTicksColor
        {
            get => _verticalAxisTicksColor;
            set
            {
                if (_verticalAxisTicksColor != value)
                {
                    _verticalAxisTicksColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _horizontalAxisTicksColor;
        public Color HorizontalAxisTicksColor
        {
            get => _horizontalAxisTicksColor;
            set
            {
                if (_horizontalAxisTicksColor != value)
                {
                    _horizontalAxisTicksColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _figureBoundsColor;
        public Color FigureBoundsColor
        {
            get => _figureBoundsColor;
            set
            {
                if (_figureBoundsColor != value)
                {
                    _figureBoundsColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private Color _figureCenterPointColor;
        public Color FigureCenterPointColor
        {
            get => _figureCenterPointColor;
            set
            {
                if (_figureCenterPointColor != value)
                {
                    _figureCenterPointColor = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _axisTickLength;
        public float AxisTickLength
        {
            get => _axisTickLength;
            set
            {
                if (_axisTickLength != value)
                {
                    _axisTickLength = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _axisThickness;
        public float AxisThickness
        {
            get => _axisThickness;
            set
            {
                if (_axisThickness != value)
                {
                    _axisThickness = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private float _axisTickThickness;
        public float AxisTickThickness
        {
            get => _axisTickThickness;
            set
            {
                if (_axisTickThickness != value)
                {
                    _axisTickThickness = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _attachGridToFigure;
        public bool AttachGridToFigure
        {
            get => _attachGridToFigure;
            set
            {
                if (_attachGridToFigure != value)
                {
                    _attachGridToFigure = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _drawGrid;
        public bool DrawGrid
        {
            get => _drawGrid;
            set
            {
                if (_drawGrid != value)
                {
                    _drawGrid = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _drawAxis;
        public bool DrawAxis
        {
            get => _drawAxis;
            set
            {
                if (_drawAxis != value)
                {
                    _drawAxis = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _drawAxisMarks;
        public bool DrawAxisMarks
        {
            get => _drawAxisMarks;
            set
            {
                if (_drawAxisMarks != value)
                {
                    _drawAxisMarks = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _drawAxisArrows;
        public bool DrawAxisArrows
        {
            get => _drawAxisArrows;
            set
            {
                if (_drawAxisArrows != value)
                {
                    _drawAxisArrows = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _drawFigureShapeBounds;
        public bool DrawFigureShapeBounds
        {
            get => _drawFigureShapeBounds;
            set
            {
                if (_drawFigureShapeBounds != value)
                {
                    _drawFigureShapeBounds = value;
                    InvokeSettingsChanged();
                }
            }
        }

        private bool _drawFigureBounds;
        public bool DrawFigureBounds
        {
            get => _drawFigureBounds;
            set
            {
                if (_drawFigureBounds != value)
                {
                    _drawFigureBounds = value;
                    InvokeSettingsChanged();
                }
            }
        }

        [JsonIgnore]
        private bool _shouldInvokeSettingsChanged;

        [JsonIgnore]
        public bool ShouldInvokeSettingsChanged
        {
            get => _shouldInvokeSettingsChanged;
            set
            {
                if (_shouldInvokeSettingsChanged != value)
                {
                    var shouldInvokeSettingsChanged = !_shouldInvokeSettingsChanged && value;

                    _shouldInvokeSettingsChanged = value;

                    if (shouldInvokeSettingsChanged)
                    {
                        SettingsChanged?.Invoke();
                    }
                }
            }
        }

        private int _canvasRefreshRate;
        public int CanvasRefreshRate
        {
            get => _canvasRefreshRate;
            set
            {
                if (_canvasRefreshRate != value)
                {
                    _canvasRefreshRate = value;
                    InvokeSettingsChanged();
                }
            }
        }
    }
}
