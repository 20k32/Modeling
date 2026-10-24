using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Curves;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using Modeling.Models.UserInterface;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    public sealed partial class DrawingViewModel : BaseViewModel
    {
        readonly IFigure _curve = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _curveTangent = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _curveNormal = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _curvePointMarker = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _curveRotationPointMarker = Ioc.Default.GetRequiredService<IFigure>();

        PointSingle _curveCenter;
        float _curvePixelsPerCentimeter;

        bool _isCurveInitialized;
        CurveDraggingTarget _curveDraggingTarget;
        PointSingle _curvePreviousDraggingPoint;
        bool _isCurveAnimationScaleIncreasing;

        float _lastCurveRotationAngle;

        CancellationTokenSource _curveAnimationCancellationSource;

        float CurvePointParameter => CurvePointParameterDegrees.DegreesToRadian();

        void InitializeCurve(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            _curveCenter = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);
            _curvePixelsPerCentimeter = pixelsPerCentimeter;

            _isCurveAnimationScaleIncreasing = true;
            _curveDraggingTarget = CurveDraggingTarget.None;

            SetCurveScaleSilent(CurveRelatedConstants.DEFAULT_SCALE_CENTIMETERS);
            SetCurvePointsCountSilent(CurveRelatedConstants.DEFAULT_POINTS_COUNT);
            SetCurvePointParameterDegreesSilent(CurveRelatedConstants.DEFAULT_POINT_PARAMETER_DEGREES);
            SetCurveTangentLengthSilent(CurveRelatedConstants.DEFAULT_TANGENT_LENGTH_CENTIMETERS);
            SetPickCurvePointByMouseSilent(false);

            ResetCurveTransformValues();

            InitializeCurveFigures(pixelsPerCentimeter);

            _isCurveInitialized = true;

            UpdateCurveState();
        }

        void ClearCurve()
        {
            _isCurveInitialized = false;

            _ = StopCurveAnimationAsync();

            _curve.Clear();
            _curveTangent.Clear();
            _curveNormal.Clear();
            _curvePointMarker.Clear();
            _curveRotationPointMarker.Clear();
        }

        void InitializeCurveFigures(float pixelsPerCentimeter)
        {
            var geometryFactory = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>();

            _curve.AddSegment(geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None), pixelsPerCentimeter);
            _curve.AddSegment(geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None), pixelsPerCentimeter);

            _curveTangent.AddSegment(geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None), pixelsPerCentimeter);
            _curveNormal.AddSegment(geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None), pixelsPerCentimeter);

            var markerCenter = DrawingConstants.DEFAULT_POINT;

            _curvePointMarker.AddSegment(
                markerCenter.GetCirclePoints(CurveRelatedConstants.POINT_MARKER_RADIUS_PIXELS).ToCirclePointGeometry(markerCenter),
                pixelsPerCentimeter);
            _curvePointMarker.CalculatePropertiesFromSegments();

            var size = CurveRelatedConstants.ROTATION_POINT_MARKER_SIZE_PIXELS;

            _curveRotationPointMarker.AddSegments([
                new[] { new PointSingle(-size, -size), new PointSingle(size, size) }.ToLinePointGeometry(),
                new[] { new PointSingle(-size, size), new PointSingle(size, -size) }.ToLinePointGeometry()],
                pixelsPerCentimeter);
            _curveRotationPointMarker.CalculatePropertiesFromSegments();
        }

        void UpdateCurveState()
        {
            UpdateCurvePoints();
            RebuildCurveTransform();
            UpdateCurvePointGeometry();
            UpdateCurveInformation();
        }

        async Task RefreshCurveAsync()
        {
            if (_isCurveInitialized)
            {
                UpdateCurveState();

                await RedrawAllAsync();
            }
        }

        async Task RefreshCurveWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(RefreshCurveAsync);
        }

        PointSingle ToCanvasPoint(PointSingle curvePoint)
            => new(
                _curveCenter.X + curvePoint.X * _curvePixelsPerCentimeter,
                _curveCenter.Y - curvePoint.Y * _curvePixelsPerCentimeter);

        PointSingle ToCurvePoint(PointSingle canvasPoint)
            => new(
                (canvasPoint.X - _curveCenter.X) / _curvePixelsPerCentimeter,
                (_curveCenter.Y - canvasPoint.Y) / _curvePixelsPerCentimeter);

        static float GetCurveSampleParameter(int index, int pointsCount)
            => BernoulliLemniscate.PARAMETER_START
            + (BernoulliLemniscate.PARAMETER_END - BernoulliLemniscate.PARAMETER_START) * index / pointsCount;

        IEnumerable<PointSingle> CreateCurvePoints(float scale, int pointsCount, int firstIndex, int lastIndex)
        {
            for (var i = firstIndex; i <= lastIndex; i++)
            {
                yield return ToCanvasPoint(BernoulliLemniscate.GetPoint(scale, GetCurveSampleParameter(i, pointsCount)));
            }
        }

        void UpdateCurvePoints()
        {
            var scale = CurveScale;
            var pointsCount = (int)CurvePointsCount;
            var halfPointsCount = pointsCount / 2;

            SetSegmentPoints(_curve.First(), CreateCurvePoints(scale, pointsCount, 0, halfPointsCount));
            SetSegmentPoints(_curve.Last(), CreateCurvePoints(scale, pointsCount, halfPointsCount, pointsCount));

            _curve.CalculatePropertiesFromSegments();
        }

        static void SetSegmentPoints(IPointGeometry segment, IEnumerable<PointSingle> points)
        {
            segment.ClearPoints();
            segment.AddPointsRange(points);
            segment.CalculateBounds();
        }

        void UpdateCurvePointGeometry()
        {
            var scale = CurveScale;
            var parameter = CurvePointParameter;

            var point = BernoulliLemniscate.GetPoint(scale, parameter);
            var halfLength = CurveTangentLength / 2;

            var tangent = ScalePoint(BernoulliLemniscate.GetUnitTangent(scale, parameter), halfLength);
            var normal = ScalePoint(BernoulliLemniscate.GetUnitNormal(scale, parameter), halfLength);

            SetSegmentPoints(_curveTangent, ToCanvasPoint(point - tangent), ToCanvasPoint(point + tangent));
            SetSegmentPoints(_curveNormal, ToCanvasPoint(point - normal), ToCanvasPoint(point + normal));
        }

        static PointSingle ScalePoint(PointSingle point, float scale)
            => new(point.X * scale, point.Y * scale);

        static void SetSegmentPoints(IFigure lineFigure, PointSingle start, PointSingle end)
        {
            var segment = lineFigure.First();

            segment.ClearPoints();
            segment.AddPoint(start);
            segment.AddPoint(end);
            segment.CalculateBounds();

            lineFigure.CalculatePropertiesFromSegments();
        }

        void RebuildCurveTransform()
        {
            _figureTranslationTransform = MatrixExtensions.CreateTranslationTransform(
                CurveTranslationX,
                CurveTranslationY);

            _figureTransform = _figureTranslationTransform * _figureRotationTransform;
        }

        void ApplyCurveRotationDelta()
        {
            var rotationAngleDelta = CurveRotationAngle - _lastCurveRotationAngle;
            _lastCurveRotationAngle = CurveRotationAngle;

            if (rotationAngleDelta == 0)
            {
                return;
            }

            RebuildCurveTransform();

            var rotationDeltaTransform = MatrixExtensions.CreateRotationTransform(
                GetCurveRotationPointOnCanvas(),
                rotationAngleDelta.DegreesToRadian());

            _figureRotationTransform = _figureTranslationTransform.Inverse()
                * rotationDeltaTransform
                * _figureTranslationTransform
                * _figureRotationTransform;
        }

        PointSingle GetCurveRotationPointOnCanvas()
            => new(CurveRotationPointX, CurveRotationPointY);

        PointSingle GetCurveInflectionPointOnCanvas()
            => _figureTransform * ToCanvasPoint(BernoulliLemniscate.GetInflectionPoint());

        void ResetCurveTransformValues()
        {
            SetCurveTranslationXSilent(0);
            SetCurveTranslationYSilent(0);
            SetCurveRotationPointXSilent(_curveCenter.X);
            SetCurveRotationPointYSilent(_curveCenter.Y);
            SetCurveRotationAngleSilent(0);

            _lastCurveRotationAngle = 0;
            _figureRotationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
        }

        PointSingle TransformCurvePoint(PointSingle curvePoint)
            => ToCurvePoint(_figureTransform * ToCanvasPoint(curvePoint));

        static string FormatNumber(float value)
            => float.IsPositiveInfinity(value) ? "∞" : value.ToString(CurveRelatedConstants.NUMBER_FORMAT);

        static string FormatPoint(PointSingle point)
            => $"({FormatNumber(point.X)}; {FormatNumber(point.Y)})";

        void UpdateCurveInformation()
        {
            var scale = CurveScale;
            var parameter = CurvePointParameter;

            var point = BernoulliLemniscate.GetPoint(scale, parameter);
            var transformedPoint = TransformCurvePoint(point);

            CurvePointCoordinatesText = $"t = {FormatNumber(CurvePointParameterDegrees)}°, P = {FormatPoint(point)} cm";
            CurveTransformedPointCoordinatesText = $"After transformations: {FormatPoint(transformedPoint)} cm";

            var slope = BernoulliLemniscate.GetTangentSlope(scale, parameter);
            CurveTangentSlopeText = float.IsPositiveInfinity(slope)
                ? "Tangent slope dy/dx = ∞ (vertical tangent)"
                : $"Tangent slope dy/dx = {FormatNumber(slope)}";

            var curvature = BernoulliLemniscate.GetCurvature(scale, parameter);
            var radius = BernoulliLemniscate.GetRadiusOfCurvature(scale, parameter);
            CurveRadiusOfCurvatureText = float.IsPositiveInfinity(radius)
                ? "Radius of curvature R = ∞ (inflection point, κ = 0)"
                : $"Radius of curvature R = {FormatNumber(radius)} cm (κ = {FormatNumber(curvature)} 1/cm)";

            var arcLengthToPoint = BernoulliLemniscate.GetArcLength(scale, BernoulliLemniscate.PARAMETER_START, parameter);
            CurveArcLengthToPointText = $"Arc length from t = 0° to t: {FormatNumber(arcLengthToPoint)} cm";

            var totalArcLength = BernoulliLemniscate.GetTotalArcLength(scale);
            CurveTotalArcLengthText = $"Total arc length: {FormatNumber(totalArcLength)} cm (2√2·ϖ·a = {FormatNumber(BernoulliLemniscate.GetTotalArcLengthClosedForm(scale))})";
            CurveLoopArcLengthText = $"One loop length: {FormatNumber(BernoulliLemniscate.GetLoopArcLength(scale))} cm";

            CurveTotalAreaText = $"Total enclosed area: {FormatNumber(BernoulliLemniscate.GetTotalArea(scale))} cm² (2a² = {FormatNumber(2 * BernoulliLemniscate.GetLoopAreaClosedForm(scale))})";
            CurveLoopAreaText = $"One loop area: {FormatNumber(BernoulliLemniscate.GetLoopArea(scale))} cm² (a² = {FormatNumber(BernoulliLemniscate.GetLoopAreaClosedForm(scale))})";

            var inflectionPoint = BernoulliLemniscate.GetInflectionPoint();
            CurveInflectionPointsText = $"Inflection point: O{FormatPoint(inflectionPoint)} at t = 90° and t = 270°; "
                + $"after transformations {FormatPoint(TransformCurvePoint(inflectionPoint))}. "
                + "The curvature changes its sign there, the curve also crosses itself (node).";

            var boundingHalfSize = BernoulliLemniscate.GetBoundingHalfSize(scale);
            CurveAsymptotesText = BernoulliLemniscate.HAS_ASYMPTOTES
                ? string.Empty
                : $"No asymptotes: the curve is bounded, |x| ≤ a√2 = {FormatNumber(boundingHalfSize.X)} cm, |y| ≤ a/2 = {FormatNumber(boundingHalfSize.Y)} cm.";
        }

        PointListTransformMessageParameter GetDrawingCurveMessage(PointListTransformMessageParameter parentMessage)
        {
            var thickness = _drawingSettingsProvider.Settings.FigureDrawingThickness;

            var result = GetDrawingCurveFigureMessage(parentMessage, _curve,
                new DrawingColor(_drawingSettingsProvider.Settings.DrawingColor),
                _figureTransform,
                thickness);

            if (_drawingSettingsProvider.Settings.DrawFigureBounds)
            {
                result = result.With(
                    points: _curve.Bounds.GetPointsFromBounds(),
                    color: new(_drawingSettingsProvider.Settings.FigureBoundsColor),
                    transformMatrix: _figureTransform);
            }

            if (CurveInflectionPointsVisible)
            {
                var inflectionPoint = ToCanvasPoint(BernoulliLemniscate.GetInflectionPoint());

                result = GetDrawingCurveFigureMessage(result, _curvePointMarker,
                    new DrawingColor(DrawingConstants.CURVE_INFLECTION_POINT_COLOR),
                    _figureTransform * MatrixExtensions.CreateTranslationTransform(inflectionPoint.X, inflectionPoint.Y),
                    shouldFillGeometry: true);
            }

            if (CurveTangentVisible)
            {
                var point = ToCanvasPoint(BernoulliLemniscate.GetPoint(CurveScale, CurvePointParameter));

                result = GetDrawingCurveFigureMessage(result, _curveTangent,
                    new DrawingColor(DrawingConstants.CURVE_TANGENT_COLOR),
                    _figureTransform);

                result = GetDrawingCurveFigureMessage(result, _curveNormal,
                    new DrawingColor(DrawingConstants.CURVE_NORMAL_COLOR),
                    _figureTransform);

                result = GetDrawingCurveFigureMessage(result, _curvePointMarker,
                    new DrawingColor(DrawingConstants.CURVE_POINT_COLOR),
                    _figureTransform * MatrixExtensions.CreateTranslationTransform(point.X, point.Y),
                    shouldFillGeometry: true);
            }

            if (CurveRotationPointVisible)
            {
                var rotationPoint = GetCurveRotationPointOnCanvas();

                result = GetDrawingCurveFigureMessage(result, _curveRotationPointMarker,
                    new DrawingColor(DrawingConstants.CURVE_ROTATION_POINT_COLOR),
                    MatrixExtensions.CreateTranslationTransform(rotationPoint.X, rotationPoint.Y));
            }

            return result;
        }

        static PointListTransformMessageParameter GetDrawingCurveFigureMessage(PointListTransformMessageParameter parentMessage,
            IFigure figure,
            DrawingColor color,
            Matrix3x3Single transformMatrix,
            float? thickness = default,
            bool shouldFillGeometry = false)
        {
            var result = parentMessage;

            foreach (var segment in figure)
            {
                result = result.With(
                    points: segment.Points,
                    color: color,
                    transformMatrix: transformMatrix,
                    shouldFillGeometry: shouldFillGeometry,
                    fillColor: color,
                    thickness: thickness);
            }

            return result;
        }

        async Task SelectNearestCurvePointAsync(PointSingle canvasPoint)
        {
            if (!_isCurveInitialized)
            {
                return;
            }

            var target = ToCurvePoint(_figureTransform.Inverse() * canvasPoint);

            var scale = CurveScale;
            var pointsCount = (int)CurvePointsCount;
            var nearestIndex = 0;
            var nearestDistance = float.MaxValue;

            for (var i = 0; i < pointsCount; i++)
            {
                var distance = BernoulliLemniscate.GetPoint(scale, GetCurveSampleParameter(i, pointsCount)).CalculateDistance(target);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            var refinementStart = GetCurveSampleParameter(nearestIndex - 1, pointsCount);
            var refinementStep = (GetCurveSampleParameter(nearestIndex + 1, pointsCount) - refinementStart)
                / CurveRelatedConstants.NEAREST_POINT_REFINEMENT_STEPS;

            var nearestParameter = GetCurveSampleParameter(nearestIndex, pointsCount);
            var nearestCurveDistance = float.MaxValue;

            for (var i = 0; i <= CurveRelatedConstants.NEAREST_POINT_REFINEMENT_STEPS; i++)
            {
                var parameter = refinementStart + i * refinementStep;
                var distance = BernoulliLemniscate.GetPoint(scale, parameter).CalculateDistance(target);

                if (distance < nearestCurveDistance)
                {
                    nearestCurveDistance = distance;
                    nearestParameter = parameter;
                }
            }

            var parameterDegrees = BernoulliLemniscate.NormalizeParameter(nearestParameter).RadianToDegrees();

            SetCurvePointParameterDegreesSilent(parameterDegrees);

            UpdateCurvePointGeometry();
            UpdateCurveInformation();

            await RedrawAllAsync();
        }

        CurveDraggingTarget FindCurveDraggingTarget(PointSingle point)
        {
            var hitDistance = CurveRelatedConstants.DRAGGING_HIT_DISTANCE_PIXELS;

            if (CurveInflectionPointsVisible && GetCurveInflectionPointOnCanvas().CalculateDistance(point) <= hitDistance)
            {
                return CurveDraggingTarget.Figure;
            }

            if (CurveRotationPointVisible && GetCurveRotationPointOnCanvas().CalculateDistance(point) <= hitDistance)
            {
                return CurveDraggingTarget.RotationPoint;
            }

            return PickCurvePointByMouse ? CurveDraggingTarget.CurvePoint : CurveDraggingTarget.None;
        }

        async Task HandleCurvePointerPressedAsync(PointSingle point)
        {
            _curveDraggingTarget = FindCurveDraggingTarget(point);
            _curvePreviousDraggingPoint = point;

            if (_curveDraggingTarget != CurveDraggingTarget.None)
            {
                ChangeMouseCursor(MouseCursor.Move);
            }

            if (_curveDraggingTarget == CurveDraggingTarget.CurvePoint)
            {
                await SelectNearestCurvePointAsync(point);
            }
        }

        async Task HandleCurvePointerMovedAsync(PointSingle point)
        {
            switch (_curveDraggingTarget)
            {
                case CurveDraggingTarget.Figure: await MoveCurveFigureAsync(point); break;
                case CurveDraggingTarget.RotationPoint: await MoveCurveRotationPointAsync(point); break;
                case CurveDraggingTarget.CurvePoint: await SelectNearestCurvePointAsync(point); break;
                default:
                    ChangeMouseCursor(FindCurveDraggingTarget(point) == CurveDraggingTarget.None
                        ? MouseCursor.Default
                        : MouseCursor.Finger);
                    break;
            }
        }

        void HandleCurvePointerReleased()
        {
            _curveDraggingTarget = CurveDraggingTarget.None;

            ChangeMouseCursor(MouseCursor.Default);
        }

        async Task MoveCurveFigureAsync(PointSingle point)
        {
            var offset = point - _curvePreviousDraggingPoint;
            _curvePreviousDraggingPoint = point;

            SetCurveTranslationXSilent(CurveTranslationX + offset.X);
            SetCurveTranslationYSilent(CurveTranslationY + offset.Y);

            RebuildCurveTransform();
            UpdateCurveInformation();

            await RedrawAllAsync();
        }

        async Task MoveCurveRotationPointAsync(PointSingle point)
        {
            SetCurveRotationPointXSilent(point.X);
            SetCurveRotationPointYSilent(point.Y);

            await RedrawAllAsync();
        }

        async Task StartCurveAnimationCoreAsync()
        {
            if (!_isCurveInitialized || IsCurveAnimationRunning)
            {
                return;
            }

            using (var animationCancellationSource = new CancellationTokenSource())
            {
                _curveAnimationCancellationSource = animationCancellationSource;

                IsCurveAnimationRunning = true;

                try
                {
                    await AnimateCurveAsync(animationCancellationSource.Token);
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
                    // a newer animation may have been started after this one was paused
                    if (_curveAnimationCancellationSource == animationCancellationSource)
                    {
                        _curveAnimationCancellationSource = default;
                        IsCurveAnimationRunning = false;
                    }
                }
            }
        }

        async Task AnimateCurveAsync(CancellationToken token)
        {
            var userInterfaceConstants = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();
            var animationTimeoutMilliseconds = userInterfaceConstants.AnimationTimeoutMilliseconds;

            while (!token.IsCancellationRequested)
            {
                SetCurveScaleSilent(GetNextAnimationScale(CurveScale));

                if (MoveCurvePointDuringAnimation && CurveTangentVisible)
                {
                    var parameterDegrees = (CurvePointParameterDegrees + CurveRelatedConstants.ANIMATION_POINT_PARAMETER_STEP_DEGREES)
                        % DrawingConstants.CIRCLE_END_ANGLE_DEGREES;

                    SetCurvePointParameterDegreesSilent(parameterDegrees);
                }

                UpdateCurvePoints();
                UpdateCurvePointGeometry();
                UpdateCurveInformation();

                await RedrawAllAsync();

                await Task.Delay(animationTimeoutMilliseconds, token);
            }
        }

        float GetNextAnimationScale(float scale)
        {
            var step = CurveRelatedConstants.ANIMATION_SCALE_STEP_CENTIMETERS;

            if (scale >= CurveRelatedConstants.ANIMATION_MAXIMUM_SCALE_CENTIMETERS)
            {
                _isCurveAnimationScaleIncreasing = false;
            }
            else if (scale <= CurveRelatedConstants.ANIMATION_MINIMUM_SCALE_CENTIMETERS)
            {
                _isCurveAnimationScaleIncreasing = true;
            }

            var nextScale = _isCurveAnimationScaleIncreasing ? scale + step : scale - step;

            return nextScale.Clamp(
                CurveRelatedConstants.ANIMATION_MINIMUM_SCALE_CENTIMETERS,
                CurveRelatedConstants.ANIMATION_MAXIMUM_SCALE_CENTIMETERS);
        }

        async Task StopCurveAnimationAsync()
        {
            var animationCancellationSource = _curveAnimationCancellationSource;

            _curveAnimationCancellationSource = default;
            IsCurveAnimationRunning = false;

            await animationCancellationSource.TryCancelAsync(shouldDispose: false);
        }

        async Task ResetCurveAnimationCoreAsync()
        {
            await StopCurveAnimationAsync();

            _isCurveAnimationScaleIncreasing = true;

            SetCurveScaleSilent(CurveRelatedConstants.DEFAULT_SCALE_CENTIMETERS);
            SetCurvePointParameterDegreesSilent(CurveRelatedConstants.DEFAULT_POINT_PARAMETER_DEGREES);

            await RefreshCurveAsync();
        }

        async Task ResetCurveTransformsCoreAsync()
        {
            ResetCurveTransformValues();

            await RefreshCurveAsync();
        }
    }
}
