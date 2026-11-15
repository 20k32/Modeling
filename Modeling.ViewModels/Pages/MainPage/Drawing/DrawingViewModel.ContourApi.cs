using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Contours;
using Modeling.Models.Drawing.Curves;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using Modeling.Models.UserInterface;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    // Lab 3: curvilinear contour (poodle) made of second order arcs in the engineering form.
    // Reference points are kept in centimeters, the origin is the canvas center, the y axis points up.
    public sealed partial class DrawingViewModel : BaseViewModel
    {
        readonly IFigure _contourCurve = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _contourPolygon = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _contourPointMarker = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _contourRotationPointMarker = Ioc.Default.GetRequiredService<IFigure>();

        // one geometry per arc, in the order of the contours and their arcs
        readonly List<IPointGeometry> _contourCurveSegments = [];
        readonly List<IPointGeometry> _contourPolygonSegments = [];

        List<CurvilinearContour> _contours = [];
        List<PointSingle[]> _contourMorphStart;
        List<PointSingle[]> _contourMorphTarget = [];

        PointSingle _contourCenter;
        float _contourPixelsPerCentimeter;

        bool _isContourInitialized;
        bool _isContourMorphForward;
        float _contourMorphProgress;

        CurveDraggingTarget _contourDraggingTarget;
        int _selectedContourIndex;
        int _selectedContourPointIndex;

        float _lastContourRotationAngle;

        // a second press on the same node within the double click time toggles a break point (Lab 4)
        long _lastContourPressTicks;
        int _lastPressedContourIndex = ContourRelatedConstants.NO_SELECTION;
        int _lastPressedContourPointIndex = ContourRelatedConstants.NO_SELECTION;

        CancellationTokenSource _contourAnimationCancellationSource;

        bool HasSelectedContourPoint => _selectedContourIndex != ContourRelatedConstants.NO_SELECTION;

        // Lab 3 builds the contour without smoothness, Lab 4 keeps the first order smoothness at the nodes
        void InitializeContour(SizeSingle canvasSize, float pixelsPerCentimeter, bool isSmoothMode)
        {
            IsSmoothContourMode = isSmoothMode;

            _contourCenter = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);
            _contourPixelsPerCentimeter = pixelsPerCentimeter;

            _contourDraggingTarget = CurveDraggingTarget.None;
            _selectedContourIndex = ContourRelatedConstants.NO_SELECTION;
            _selectedContourPointIndex = ContourRelatedConstants.NO_SELECTION;
            _lastPressedContourIndex = ContourRelatedConstants.NO_SELECTION;
            _lastPressedContourPointIndex = ContourRelatedConstants.NO_SELECTION;

            _contours = PoodleContour.Create();
            _contourMorphTarget = PoodleContour.CreateMorphTargets();
            ResetContourMorphState();

            SetContourSegmentPointsCountSilent(ContourRelatedConstants.DEFAULT_SEGMENT_POINTS_COUNT);
            SetContourControlWeightSilent(ContourRelatedConstants.DEFAULT_WEIGHT);
            SetSelectedContourPointWeightSilent(ContourRelatedConstants.DEFAULT_WEIGHT);

            ResetContourTransformValues();

            InitializeContourFigures(pixelsPerCentimeter);

            _isContourInitialized = true;
            _drawingAction = UserPointDrawingAction.ContourPointSelection;

            UpdateContourState();
        }

        void ClearContour()
        {
            _isContourInitialized = false;

            if (_drawingAction == UserPointDrawingAction.ContourPointSelection)
            {
                _drawingAction = UserPointDrawingAction.None;
            }

            _ = StopContourAnimationAsync();

            _contourCurve.Clear();
            _contourPolygon.Clear();
            _contourPointMarker.Clear();
            _contourRotationPointMarker.Clear();

            _contourCurveSegments.Clear();
            _contourPolygonSegments.Clear();
        }

        void InitializeContourFigures(float pixelsPerCentimeter)
        {
            var geometryFactory = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>();

            foreach (var contour in _contours)
            {
                for (var segment = 0; segment < contour.SegmentsCount; segment++)
                {
                    var curveSegment = geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None);
                    var polygonSegment = geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None);

                    _contourCurveSegments.Add(curveSegment);
                    _contourPolygonSegments.Add(polygonSegment);

                    _contourCurve.AddSegment(curveSegment, pixelsPerCentimeter);
                    _contourPolygon.AddSegment(polygonSegment, pixelsPerCentimeter);
                }
            }

            // markers are built around (0; 0) once and moved to their place with a transform matrix
            var markerCenter = DrawingConstants.DEFAULT_POINT;

            _contourPointMarker.AddSegment(
                markerCenter.GetCirclePoints(ContourRelatedConstants.POINT_MARKER_RADIUS_PIXELS).ToCirclePointGeometry(markerCenter),
                pixelsPerCentimeter);
            _contourPointMarker.CalculatePropertiesFromSegments();

            var size = ContourRelatedConstants.ROTATION_POINT_MARKER_SIZE_PIXELS;

            _contourRotationPointMarker.AddSegments([
                new[] { new PointSingle(-size, -size), new PointSingle(size, size) }.ToLinePointGeometry(),
                new[] { new PointSingle(-size, size), new PointSingle(size, -size) }.ToLinePointGeometry()],
                pixelsPerCentimeter);
            _contourRotationPointMarker.CalculatePropertiesFromSegments();
        }

        void UpdateContourState()
        {
            UpdateContourGeometry();
            RebuildContourTransform();
            UpdateContourInformation();
        }

        async Task RefreshContourAsync()
        {
            if (_isContourInitialized)
            {
                UpdateContourState();

                await RedrawAllAsync();
            }
        }

        async Task RefreshContourWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(RefreshContourAsync);
        }

        PointSingle ToContourCanvasPoint(PointSingle contourPoint)
            => new(
                _contourCenter.X + contourPoint.X * _contourPixelsPerCentimeter,
                _contourCenter.Y - contourPoint.Y * _contourPixelsPerCentimeter);

        PointSingle ToContourPoint(PointSingle canvasPoint)
            => new(
                (canvasPoint.X - _contourCenter.X) / _contourPixelsPerCentimeter,
                (_contourCenter.Y - canvasPoint.Y) / _contourPixelsPerCentimeter);

        IEnumerable<PointSingle> CreateContourSegmentPoints(CurvilinearContour contour, int segment, int pointsCount)
        {
            for (var i = 0; i <= pointsCount; i++)
            {
                var u = EngineeringQuadraticCurve.PARAMETER_START
                    + (EngineeringQuadraticCurve.PARAMETER_END - EngineeringQuadraticCurve.PARAMETER_START) * i / pointsCount;

                yield return ToContourCanvasPoint(contour.GetSegmentPoint(segment, u));
            }
        }

        IEnumerable<PointSingle> CreateContourPolygonPoints(CurvilinearContour contour, int segment)
        {
            var start = contour.GetSegmentStartIndex(segment);

            yield return ToContourCanvasPoint(contour.GetPoint(start));
            yield return ToContourCanvasPoint(contour.GetPoint(start + 1));
            yield return ToContourCanvasPoint(contour.GetPoint(contour.GetSegmentEndIndex(segment)));
        }

        // every arc is sampled from u = 0 to u = 1, its characteristic polygon is the broken line A-B-C
        void UpdateContourGeometry()
        {
            var pointsCount = (int)ContourSegmentPointsCount;
            var geometryIndex = 0;

            foreach (var contour in _contours)
            {
                for (var segment = 0; segment < contour.SegmentsCount; segment++)
                {
                    SetSegmentPoints(_contourCurveSegments[geometryIndex], CreateContourSegmentPoints(contour, segment, pointsCount));
                    SetSegmentPoints(_contourPolygonSegments[geometryIndex], CreateContourPolygonPoints(contour, segment));

                    geometryIndex++;
                }
            }

            _contourCurve.CalculatePropertiesFromSegments();
            _contourPolygon.CalculatePropertiesFromSegments();
        }

        void RebuildContourTransform()
        {
            _figureTranslationTransform = MatrixExtensions.CreateTranslationTransform(
                ContourTranslationX,
                ContourTranslationY);

            _figureTransform = _figureTranslationTransform * _figureRotationTransform;
        }

        // the same incremental rotation as in Lab 2: only the angle difference is applied around the current rotation point
        void ApplyContourRotationDelta()
        {
            var rotationAngleDelta = ContourRotationAngle - _lastContourRotationAngle;
            _lastContourRotationAngle = ContourRotationAngle;

            if (rotationAngleDelta == 0)
            {
                return;
            }

            RebuildContourTransform();

            var rotationDeltaTransform = MatrixExtensions.CreateRotationTransform(
                GetContourRotationPointOnCanvas(),
                rotationAngleDelta.DegreesToRadian());

            _figureRotationTransform = _figureTranslationTransform.Inverse()
                * rotationDeltaTransform
                * _figureTranslationTransform
                * _figureRotationTransform;
        }

        PointSingle GetContourRotationPointOnCanvas()
            => new(ContourRotationPointX, ContourRotationPointY);

        void ResetContourTransformValues()
        {
            SetContourTranslationXSilent(0);
            SetContourTranslationYSilent(0);
            SetContourRotationPointXSilent(_contourCenter.X);
            SetContourRotationPointYSilent(_contourCenter.Y);
            SetContourRotationAngleSilent(0);

            _lastContourRotationAngle = 0;
            _figureRotationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
        }

        static string FormatContourNumber(float value) => value.ToString(ContourRelatedConstants.NUMBER_FORMAT);

        static string GetArcTypeName(ConicArcType arcType) => arcType switch
        {
            ConicArcType.Ellipse => "ellipse",
            ConicArcType.Parabola => "parabola",
            _ => "hyperbola",
        };

        void UpdateContourInformation()
        {
            var segmentsCount = 0;
            var pointsCount = 0;

            foreach (var contour in _contours)
            {
                segmentsCount += contour.SegmentsCount;
                pointsCount += contour.PointsCount;
            }

            ContourInfoText = IsSmoothContourMode
                ? $"Contours: {_contours.Count}, arcs: {segmentsCount}, nodes: {pointsCount / 2} (break points: {CountContourBreakPoints()}), reference points: {pointsCount / 2}"
                : $"Contours: {_contours.Count}, arcs: {segmentsCount}, reference points: {pointsCount}";
            ContourMorphProgressText = $"Transition to the smiley: {Math.Round(_contourMorphProgress * 100)}%";

            if (!HasSelectedContourPoint)
            {
                ContourSelectedPointText = "No point selected: click a reference point on the canvas";
                ContourSelectedSegmentText = string.Empty;
                ContourSmoothnessText = string.Empty;
                SetSelectedContourPointNodeStateSilent(isNode: false, isBreakPoint: false);
                return;
            }

            var selectedContour = _contours[_selectedContourIndex];
            var pointIndex = _selectedContourPointIndex;
            var point = selectedContour.GetPoint(pointIndex);
            var pointKind = IsSmoothContourMode
                ? CurvilinearContour.IsControlPoint(pointIndex) ? "reference point" : selectedContour.IsBreakPoint(pointIndex) ? "break point" : "node"
                : CurvilinearContour.IsControlPoint(pointIndex) ? "control point B" : "arc end A";

            ContourSelectedPointText = $"Selected: {PoodleContour.CONTOUR_NAMES[_selectedContourIndex]}, point {pointIndex} ({pointKind}), "
                + $"P = ({FormatContourNumber(point.X)}; {FormatContourNumber(point.Y)}) cm, w = {FormatContourNumber(selectedContour.GetWeight(pointIndex))}";

            var segment = selectedContour.GetSegmentOfPoint(pointIndex);
            var start = selectedContour.GetSegmentStartIndex(segment);
            var end = selectedContour.GetSegmentEndIndex(segment);

            ContourSelectedSegmentText = $"Arc {segment}: wA = {FormatContourNumber(selectedContour.GetWeight(start))}, "
                + $"wB = {FormatContourNumber(selectedContour.GetWeight(start + 1))}, wC = {FormatContourNumber(selectedContour.GetWeight(end))}, "
                + $"f = {FormatContourNumber(selectedContour.GetSegmentDiscriminant(segment))} — arc of {GetArcTypeName(selectedContour.GetSegmentArcType(segment))}";

            UpdateContourSmoothnessInformation(selectedContour, pointIndex);
        }

        // for a node: is the smoothness kept there and the angle between the tangents of the two arcs
        void UpdateContourSmoothnessInformation(CurvilinearContour contour, int pointIndex)
        {
            var isNode = CurvilinearContour.IsNode(pointIndex);

            SetSelectedContourPointNodeStateSilent(isNode, contour.IsBreakPoint(pointIndex));

            if (!IsSmoothContourMode)
            {
                ContourSmoothnessText = string.Empty;
                return;
            }

            if (!isNode)
            {
                ContourSmoothnessText = "Reference point: moving it turns the opposite reference points around the smooth nodes next to it";
                return;
            }

            var state = contour.IsBreakPoint(pointIndex) ? "break point, smoothness is not kept" : "smooth node (G1), smoothness is kept";

            ContourSmoothnessText = $"Node: {state}; angle between the tangents of the arcs: {FormatContourNumber(contour.GetTangentBreakAngle(pointIndex))}°";
        }

        int CountContourBreakPoints()
        {
            var result = 0;

            foreach (var contour in _contours)
            {
                for (var i = 0; i < contour.PointsCount; i += 2)
                {
                    if (contour.IsBreakPoint(i))
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        PointListTransformMessageParameter GetDrawingContourMessage(PointListTransformMessageParameter parentMessage)
        {
            var result = GetDrawingCurveFigureMessage(parentMessage, _contourCurve,
                new DrawingColor(_drawingSettingsProvider.Settings.DrawingColor),
                _figureTransform,
                _drawingSettingsProvider.Settings.FigureDrawingThickness);

            if (_drawingSettingsProvider.Settings.DrawFigureBounds)
            {
                result = result.With(
                    points: _contourCurve.Bounds.GetPointsFromBounds(),
                    color: new(_drawingSettingsProvider.Settings.FigureBoundsColor),
                    transformMatrix: _figureTransform);
            }

            if (ContourPolygonVisible)
            {
                result = GetDrawingCurveFigureMessage(result, _contourPolygon,
                    new DrawingColor(DrawingConstants.CONTOUR_POLYGON_COLOR),
                    _figureTransform,
                    DrawingConstants.GRID_DRAWING_THICKNESS);

                result = GetDrawingContourPointsMessage(result);
            }

            if (ContourRotationPointVisible)
            {
                var rotationPoint = GetContourRotationPointOnCanvas();

                result = GetDrawingCurveFigureMessage(result, _contourRotationPointMarker,
                    new DrawingColor(DrawingConstants.CURVE_ROTATION_POINT_COLOR),
                    MatrixExtensions.CreateTranslationTransform(rotationPoint.X, rotationPoint.Y));
            }

            return result;
        }

        // arc ends and control points get different colors, the selected point is highlighted
        PointListTransformMessageParameter GetDrawingContourPointsMessage(PointListTransformMessageParameter parentMessage)
        {
            // Lab 4 colors follow the task: reference points are green, break points are red
            var anchorColor = new DrawingColor(DrawingConstants.CONTOUR_ANCHOR_POINT_COLOR);
            var breakPointColor = new DrawingColor(DrawingConstants.CONTOUR_BREAK_POINT_COLOR);
            var controlColor = new DrawingColor(IsSmoothContourMode
                ? DrawingConstants.CONTOUR_REFERENCE_POINT_COLOR
                : DrawingConstants.CONTOUR_CONTROL_POINT_COLOR);
            var selectedColor = new DrawingColor(IsSmoothContourMode
                ? DrawingConstants.CONTOUR_SELECTED_POINT_COLOR
                : DrawingConstants.CURVE_POINT_COLOR);

            var result = parentMessage;

            for (var contourIndex = 0; contourIndex < _contours.Count; contourIndex++)
            {
                var contour = _contours[contourIndex];

                for (var pointIndex = 0; pointIndex < contour.PointsCount; pointIndex++)
                {
                    var point = ToContourCanvasPoint(contour.GetPoint(pointIndex));
                    var isSelected = contourIndex == _selectedContourIndex && pointIndex == _selectedContourPointIndex;

                    var color = isSelected
                        ? selectedColor
                        : CurvilinearContour.IsControlPoint(pointIndex) ? controlColor
                        : IsSmoothContourMode && contour.IsBreakPoint(pointIndex) ? breakPointColor : anchorColor;

                    result = GetDrawingCurveFigureMessage(result, _contourPointMarker,
                        color,
                        _figureTransform * MatrixExtensions.CreateTranslationTransform(point.X, point.Y),
                        shouldFillGeometry: true);
                }
            }

            return result;
        }

        // reference points are found only while the characteristic polygon is shown, they have priority over the rotation point
        CurveDraggingTarget FindContourDraggingTarget(PointSingle point, out int contourIndex, out int pointIndex)
        {
            contourIndex = ContourRelatedConstants.NO_SELECTION;
            pointIndex = ContourRelatedConstants.NO_SELECTION;

            var hitDistance = ContourRelatedConstants.DRAGGING_HIT_DISTANCE_PIXELS;

            if (ContourPolygonVisible)
            {
                var nearestDistance = float.MaxValue;

                for (var i = 0; i < _contours.Count; i++)
                {
                    var contour = _contours[i];

                    for (var j = 0; j < contour.PointsCount; j++)
                    {
                        var distance = (_figureTransform * ToContourCanvasPoint(contour.GetPoint(j))).CalculateDistance(point);

                        if (distance <= hitDistance && distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            contourIndex = i;
                            pointIndex = j;
                        }
                    }
                }

                if (contourIndex != ContourRelatedConstants.NO_SELECTION)
                {
                    return CurveDraggingTarget.ReferencePoint;
                }
            }

            if (ContourRotationPointVisible && GetContourRotationPointOnCanvas().CalculateDistance(point) <= hitDistance)
            {
                return CurveDraggingTarget.RotationPoint;
            }

            return CurveDraggingTarget.None;
        }

        async Task HandleContourPointerPressedAsync(PointSingle point)
        {
            _contourDraggingTarget = FindContourDraggingTarget(point, out var contourIndex, out var pointIndex);

            if (_contourDraggingTarget == CurveDraggingTarget.None)
            {
                return;
            }

            ChangeMouseCursor(MouseCursor.Move);

            if (_contourDraggingTarget == CurveDraggingTarget.ReferencePoint
                && IsSmoothContourMode
                && IsContourDoubleClick(contourIndex, pointIndex)
                && CurvilinearContour.IsNode(pointIndex))
            {
                _contourDraggingTarget = CurveDraggingTarget.None;

                await SetContourBreakPointAsync(contourIndex, pointIndex, !_contours[contourIndex].IsBreakPoint(pointIndex));
                return;
            }

            if (_contourDraggingTarget == CurveDraggingTarget.ReferencePoint)
            {
                // an edited contour becomes the new start of the transition
                await StopContourAnimationAsync();
                ResetContourMorphState();

                _selectedContourIndex = contourIndex;
                _selectedContourPointIndex = pointIndex;

                SetSelectedContourPointWeightSilent(_contours[contourIndex].GetWeight(pointIndex));

                UpdateContourInformation();

                await RedrawAllAsync();
            }
        }

        async Task HandleContourPointerMovedAsync(PointSingle point)
        {
            switch (_contourDraggingTarget)
            {
                case CurveDraggingTarget.ReferencePoint: await MoveContourReferencePointAsync(point); break;
                case CurveDraggingTarget.RotationPoint: await MoveContourRotationPointAsync(point); break;
                default:
                    ChangeMouseCursor(FindContourDraggingTarget(point, out _, out _) == CurveDraggingTarget.None
                        ? MouseCursor.Default
                        : MouseCursor.Finger);
                    break;
            }
        }

        bool IsContourDoubleClick(int contourIndex, int pointIndex)
        {
            var ticks = Environment.TickCount64;

            var isDoubleClick = contourIndex == _lastPressedContourIndex
                && pointIndex == _lastPressedContourPointIndex
                && ticks - _lastContourPressTicks <= ContourRelatedConstants.DOUBLE_CLICK_TIME_MILLISECONDS;

            // the second click of a double click does not start a new one
            _lastContourPressTicks = isDoubleClick ? 0 : ticks;
            _lastPressedContourIndex = contourIndex;
            _lastPressedContourPointIndex = pointIndex;

            return isDoubleClick;
        }

        // a node that becomes smooth gets the smoothness right away: the reference point after it is turned to the line of the one before it
        async Task SetContourBreakPointAsync(int contourIndex, int pointIndex, bool isBreakPoint)
        {
            var contour = _contours[contourIndex];

            contour.SetBreakPoint(pointIndex, isBreakPoint);

            if (!isBreakPoint)
            {
                contour.EnforceSmoothness(pointIndex);
            }

            _selectedContourIndex = contourIndex;
            _selectedContourPointIndex = pointIndex;

            await StopContourAnimationAsync();
            ResetContourMorphState();

            await RefreshContourAsync();
        }

        async Task SetAllContourNodesBreakPointsAsync(bool isBreakPoint)
        {
            foreach (var contour in _contours)
            {
                for (var i = 0; i < contour.PointsCount; i += 2)
                {
                    contour.SetBreakPoint(i, isBreakPoint);

                    if (!isBreakPoint)
                    {
                        contour.EnforceSmoothness(i);
                    }
                }
            }

            await StopContourAnimationAsync();
            ResetContourMorphState();

            await RefreshContourAsync();
        }

        void HandleContourPointerReleased()
        {
            _contourDraggingTarget = CurveDraggingTarget.None;

            ChangeMouseCursor(MouseCursor.Default);
        }

        // the pointer is brought back into the untransformed contour space before it becomes the new reference point
        async Task MoveContourReferencePointAsync(PointSingle point)
        {
            var contourPoint = ToContourPoint(_figureTransform.Inverse() * point);

            var contour = _contours[_selectedContourIndex];

            if (IsSmoothContourMode)
            {
                contour.MovePointWithSmoothness(_selectedContourPointIndex, contourPoint);
            }
            else
            {
                contour.SetPoint(_selectedContourPointIndex, contourPoint);
            }

            UpdateContourGeometry();
            UpdateContourInformation();

            await RedrawAllAsync();
        }

        async Task MoveContourRotationPointAsync(PointSingle point)
        {
            SetContourRotationPointXSilent(point.X);
            SetContourRotationPointYSilent(point.Y);

            await RedrawAllAsync();
        }

        void ApplyContourControlWeight(float weight)
        {
            foreach (var contour in _contours)
            {
                for (var i = 1; i < contour.PointsCount; i += 2)
                {
                    contour.SetWeight(i, weight);
                }
            }

            if (HasSelectedContourPoint && CurvilinearContour.IsControlPoint(_selectedContourPointIndex))
            {
                SetSelectedContourPointWeightSilent(weight);
            }
        }

        void ApplySelectedContourPointWeight(float weight)
        {
            if (HasSelectedContourPoint)
            {
                _contours[_selectedContourIndex].SetWeight(_selectedContourPointIndex, weight);
            }
        }

        void ResetContourMorphState()
        {
            _contourMorphStart = default;
            _contourMorphProgress = 0;
            _isContourMorphForward = true;
        }

        // every reference point moves along its own straight line from the start position to the target one
        void ApplyContourMorph()
        {
            for (var i = 0; i < _contours.Count && i < _contourMorphTarget.Count; i++)
            {
                var start = _contourMorphStart[i];
                var target = _contourMorphTarget[i];
                var contour = _contours[i];

                for (var j = 0; j < contour.PointsCount && j < start.Length && j < target.Length; j++)
                {
                    contour.SetPoint(j, new PointSingle(
                        start[j].X + (target[j].X - start[j].X) * _contourMorphProgress,
                        start[j].Y + (target[j].Y - start[j].Y) * _contourMorphProgress));
                }

                // straight paths of the points do not keep the nodes on the lines of their reference points
                if (IsSmoothContourMode)
                {
                    contour.ProjectSmoothNodes();
                }
            }
        }

        async Task StartContourAnimationCoreAsync()
        {
            if (!_isContourInitialized || IsContourAnimationRunning)
            {
                return;
            }

            if (_contourMorphStart is null)
            {
                _contourMorphStart = _contours.ConvertAll(contour => contour.CopyPoints());
            }

            using (var animationCancellationSource = new CancellationTokenSource())
            {
                _contourAnimationCancellationSource = animationCancellationSource;

                IsContourAnimationRunning = true;

                try
                {
                    await AnimateContourAsync(animationCancellationSource.Token);
                }
                catch (Exception ex)
                {
                    if (ex is not OperationCanceledException)
                    {
                        Logger.Exception(ex);
                    }
                }
                finally
                {
                    if (_contourAnimationCancellationSource == animationCancellationSource)
                    {
                        _contourAnimationCancellationSource = default;
                        IsContourAnimationRunning = false;
                    }
                }
            }
        }

        // ping-pong between the poodle (progress 0) and the smiley (progress 1)
        async Task AnimateContourAsync(CancellationToken token)
        {
            var userInterfaceConstants = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();
            var animationTimeoutMilliseconds = userInterfaceConstants.AnimationTimeoutMilliseconds;

            while (!token.IsCancellationRequested)
            {
                var step = _isContourMorphForward
                    ? ContourRelatedConstants.MORPH_PROGRESS_STEP
                    : -ContourRelatedConstants.MORPH_PROGRESS_STEP;

                _contourMorphProgress = (_contourMorphProgress + step).Clamp(0, 1);

                if (_contourMorphProgress is 0 or 1)
                {
                    _isContourMorphForward = !_isContourMorphForward;
                }

                ApplyContourMorph();
                UpdateContourGeometry();
                UpdateContourInformation();

                await RedrawAllAsync();

                await Task.Delay(animationTimeoutMilliseconds, token);
            }
        }

        async Task StopContourAnimationAsync()
        {
            var animationCancellationSource = _contourAnimationCancellationSource;

            _contourAnimationCancellationSource = default;
            IsContourAnimationRunning = false;

            await animationCancellationSource.TryCancelAsync(shouldDispose: false);
        }

        async Task ResetContourAnimationCoreAsync()
        {
            await StopContourAnimationAsync();

            if (_contourMorphStart is not null)
            {
                for (var i = 0; i < _contours.Count && i < _contourMorphStart.Count; i++)
                {
                    _contours[i].SetPoints(_contourMorphStart[i]);
                }
            }

            ResetContourMorphState();

            await RefreshContourAsync();
        }

        async Task ResetContourTransformsCoreAsync()
        {
            ResetContourTransformValues();

            await RefreshContourAsync();
        }
    }
}
