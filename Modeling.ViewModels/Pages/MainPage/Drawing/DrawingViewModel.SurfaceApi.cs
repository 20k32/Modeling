using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Contours;
using Modeling.Models.Drawing.Curves;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Drawing.Projection;
using Modeling.Models.Drawing.Surfaces;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using Modeling.Models.UserInterface;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    // Lab 6: surface of the variant 14 (the upper part is a concave ellipsoid, the lower one is a sphere) built as a UV grid,
    // the curvilinear contour of the lab 3 lies on it, both are shown with the three point perspective of the lab 5.
    public sealed partial class DrawingViewModel : BaseViewModel
    {
        readonly IFigure _surfaceGrid = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _surfaceContour = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _surfaceContourPivotMarker = Ioc.Default.GetRequiredService<IFigure>();

        readonly List<IPointGeometry> _surfaceGridGeometries = [];
        readonly List<IPointGeometry> _surfaceContourGeometries = [];

        List<CurvilinearContour> _surfaceContours = [];

        bool _isSurfaceInitialized;
        bool _isSurfaceAnimationIncreasing;
        bool _isSurfaceAnimationDepthIncreasing;
        int _surfaceGridUCount;
        int _surfaceGridHalfVCount;

        PointSingle _surfaceContourPivotOnPlane;

        // values restored by the animation reset
        float _surfaceAnimationStartRadius;
        float _surfaceAnimationStartDepth;
        bool _hasSurfaceAnimationStart;

        CancellationTokenSource _surfaceAnimationCancellationSource;

        bool IsSpatialSceneInitialized => _isPolyhedronInitialized || _isSurfaceInitialized;

        void InitializeSurface(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            // the projection plane, the axes and the markers are the same as in the lab 5
            _polyhedronCenter = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);
            _polyhedronPixelsPerCentimeter = pixelsPerCentimeter;
            _isSurfaceAnimationIncreasing = true;
            _isSurfaceAnimationDepthIncreasing = true;
            _hasSurfaceAnimationStart = false;

            ResetSurfaceParameterValues();
            ResetSurfaceTransformValues();
            ResetSurfaceContourTransformValues();
            ResetSurfaceProjectionValues();
            ResetSurfaceAnimationValues();

            InitializePolyhedronFigures(pixelsPerCentimeter);
            InitializeSurfaceFigures(pixelsPerCentimeter);

            _isSurfaceInitialized = true;

            UpdateSurfaceState();
        }

        void ClearSurface()
        {
            _isSurfaceInitialized = false;

            _ = StopSurfaceAnimationAsync();

            _surfaceGrid.Clear();
            _surfaceContour.Clear();
            _surfaceContourPivotMarker.Clear();

            _surfaceGridGeometries.Clear();
            _surfaceContourGeometries.Clear();
            _surfaceGridUCount = 0;
            _surfaceGridHalfVCount = 0;
        }

        void InitializeSurfaceFigures(float pixelsPerCentimeter)
        {
            var geometryFactory = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>();

            _surfaceContours = PoodleContour.Create();

            foreach (var contour in _surfaceContours)
            {
                for (var segment = 0; segment < contour.SegmentsCount; segment++)
                {
                    for (var piece = 0; piece < 2; piece++)
                    {
                        var geometry = geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None);

                        _surfaceContourGeometries.Add(geometry);
                        _surfaceContour.AddSegment(geometry, pixelsPerCentimeter);
                    }
                }
            }

            var size = ContourRelatedConstants.ROTATION_POINT_MARKER_SIZE_PIXELS;

            _surfaceContourPivotMarker.AddSegments([
                new[] { new PointSingle(-size, -size), new PointSingle(size, size) }.ToLinePointGeometry(),
                new[] { new PointSingle(-size, size), new PointSingle(size, -size) }.ToLinePointGeometry()],
                pixelsPerCentimeter);
            _surfaceContourPivotMarker.CalculatePropertiesFromSegments();
        }

        // rings (u lines) are split in two halves, so that no polyline repeats its first point; meridians (v lines) go from pole to pole
        void EnsureSurfaceGridGeometries(int uCount, int halfVCount)
        {
            if (_surfaceGridUCount == uCount && _surfaceGridHalfVCount == halfVCount)
            {
                return;
            }

            _surfaceGrid.Clear();
            _surfaceGridGeometries.Clear();

            var geometryFactory = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>();
            var geometriesCount = 2 * GetSurfaceRingsCount(halfVCount) + uCount;

            for (var i = 0; i < geometriesCount; i++)
            {
                var geometry = geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None);

                _surfaceGridGeometries.Add(geometry);
                _surfaceGrid.AddSegment(geometry, _polyhedronPixelsPerCentimeter);
            }

            _surfaceGridUCount = uCount;
            _surfaceGridHalfVCount = halfVCount;
        }

        // the poles are single points, they have no ring
        static int GetSurfaceRingsCount(int halfVCount) => 2 * halfVCount;

        async Task RefreshSurfaceAsync()
        {
            if (_isSurfaceInitialized)
            {
                UpdateSurfaceState();

                await RedrawAllAsync();
            }
        }

        async Task RefreshSurfaceWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(RefreshSurfaceAsync);
        }

        // the projection parameters and the helpers (axes, vanishing points) are shared by the labs 5 and 6
        async Task RefreshProjectionSceneAsync()
        {
            if (_isPolyhedronInitialized)
            {
                await RefreshPolyhedronAsync();
            }
            else if (_isSurfaceInitialized)
            {
                await RefreshSurfaceAsync();
            }
        }

        async Task RefreshProjectionSceneWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(RefreshProjectionSceneAsync);
        }

        ConcaveTopEllipsoidSurface CreateSurface() => new(SurfaceRadius, SurfaceDepthCoefficient);

        SurfaceContourMapping CreateSurfaceContourMapping()
            => new(
                SurfaceRelatedConstants.CONTOUR_BASE_U_DEGREES,
                SurfaceRelatedConstants.CONTOUR_BASE_V_DEGREES,
                SurfaceContourShiftU,
                SurfaceContourShiftV,
                SurfaceContourAngle,
                SurfaceContourScale);

        Matrix4x4 GetSurfaceWorldTransform()
            => SpatialTransforms.Create(
                SurfaceRotationX,
                SurfaceRotationY,
                SurfaceRotationZ,
                new Vector3(SurfaceTranslationX, SurfaceTranslationY, SurfaceTranslationZ));

        // a polyline is drawn only when all of its points are in front of the centers of projection
        void SetProjectedPolyline(IPointGeometry geometry, IEnumerable<Vector3> points, Matrix4x4 world, Matrix4x4 projection)
        {
            geometry.ClearPoints();

            foreach (var point in points)
            {
                if (!ThreePointPerspective.TryProject(HomogeneousTransforms.TransformPoint(point, world), projection, out var projectedPoint))
                {
                    geometry.ClearPoints();

                    break;
                }

                geometry.AddPoint(ToProjectionCanvasPoint(projectedPoint));
            }

            geometry.CalculateBounds();
        }

        static IEnumerable<Vector3> GetGridRing(Vector3[,] grid, int row, int fromColumn, int toColumn)
        {
            var uCount = grid.GetLength(1);

            for (var column = fromColumn; column <= toColumn; column++)
            {
                yield return grid[row, column % uCount];
            }
        }

        static IEnumerable<Vector3> GetGridMeridian(Vector3[,] grid, int column)
        {
            for (var row = 0; row < grid.GetLength(0); row++)
            {
                yield return grid[row, column];
            }
        }

        // every point of the contour: point of the lab 3 curve (engineering quadratic arc) -> UV coordinates -> equation of the surface.
        // The surface is broken at the equator (z jumps from 0 to a), so an arc that crosses it is split into two polylines
        List<List<Vector3>> CreateSurfaceContourPieces(ConcaveTopEllipsoidSurface surface, SurfaceContourMapping mapping, CurvilinearContour contour, int segment)
        {
            var pointsCount = SurfaceRelatedConstants.CONTOUR_ARC_POINTS_COUNT;
            var result = new List<List<Vector3>>();
            var isPreviousUpperPart = false;

            for (var i = 0; i <= pointsCount; i++)
            {
                var u = EngineeringQuadraticCurve.PARAMETER_START
                    + (EngineeringQuadraticCurve.PARAMETER_END - EngineeringQuadraticCurve.PARAMETER_START) * i / pointsCount;

                var uv = mapping.ToUv(contour.GetSegmentPoint(segment, u));
                var isUpperPart = surface.IsUpperPart(uv.X, uv.Y);

                if (i == 0 || isUpperPart != isPreviousUpperPart)
                {
                    result.Add([]);
                }

                result[^1].Add(surface.GetPoint(uv.X, uv.Y));
                isPreviousUpperPart = isUpperPart;
            }

            return result;
        }

        // the grid and the contour are built in the own coordinates of the surface, then every point is multiplied
        // by the matrix of the euclidean transformations and by the projection matrix
        void UpdateSurfaceState()
        {
            var surface = CreateSurface();
            var world = GetSurfaceWorldTransform();
            var projection = GetProjectionMatrix();

            var uCount = ConcaveTopEllipsoidSurface.GetUCount(SurfaceMeshStepU);
            var halfVCount = ConcaveTopEllipsoidSurface.GetHalfVCount(SurfaceMeshStepV);

            EnsureSurfaceGridGeometries(uCount, halfVCount);

            var grid = surface.CreateGrid(uCount, halfVCount);
            var geometryIndex = 0;
            var halfU = uCount / 2;

            for (var row = 0; row < grid.GetLength(0); row++)
            {
                // the first and the last rows are the poles
                if (row == 0 || row == grid.GetLength(0) - 1)
                {
                    continue;
                }

                SetProjectedPolyline(_surfaceGridGeometries[geometryIndex++], GetGridRing(grid, row, 0, halfU), world, projection);
                SetProjectedPolyline(_surfaceGridGeometries[geometryIndex++], GetGridRing(grid, row, halfU, uCount), world, projection);
            }

            for (var column = 0; column < uCount; column++)
            {
                SetProjectedPolyline(_surfaceGridGeometries[geometryIndex++], GetGridMeridian(grid, column), world, projection);
            }

            _surfaceGrid.CalculatePropertiesFromSegments();

            var mapping = CreateSurfaceContourMapping();
            var contourGeometryIndex = 0;

            foreach (var contour in _surfaceContours)
            {
                for (var segment = 0; segment < contour.SegmentsCount; segment++)
                {
                    var pieces = CreateSurfaceContourPieces(surface, mapping, contour, segment);

                    // two polylines per arc: the second one is empty unless the arc crosses the equator
                    // (a third crossing inside one short arc is joined to the second piece)
                    SetProjectedPolyline(_surfaceContourGeometries[contourGeometryIndex++], pieces[0], world, projection);
                    SetProjectedPolyline(_surfaceContourGeometries[contourGeometryIndex++],
                        pieces.Count > 1 ? pieces.GetRange(1, pieces.Count - 1).SelectMany(piece => piece) : [], world, projection);
                }
            }

            _surfaceContour.CalculatePropertiesFromSegments();

            var contourPivot = HomogeneousTransforms.TransformPoint(surface.GetPoint(mapping.Pivot.X, mapping.Pivot.Y), world);

            _surfaceContourPivotOnPlane = ThreePointPerspective.TryProject(contourPivot, projection, out var projectedContourPivot)
                ? ToProjectionCanvasPoint(projectedContourPivot)
                : DrawingConstants.BREAK_POINT;

            var ownAxisLength = PolyhedronRelatedConstants.OWN_AXIS_LENGTH_RATIO * surface.Radius;
            var pivot = UpdateSpatialSceneHelpers(world, ownAxisLength, projection);

            UpdateSurfaceInformation(uCount, halfVCount, pivot, mapping);
        }

        static string FormatSurfaceNumber(float value) => value.ToString(SurfaceRelatedConstants.NUMBER_FORMAT);

        void UpdateSurfaceInformation(int uCount, int halfVCount, Vector3 pivot, SurfaceContourMapping mapping)
        {
            SurfaceInfoText = $"UV grid: {uCount} × {2 * halfVCount + 2} points, step u = {FormatSurfaceNumber(360f / uCount)}°, step v = {FormatSurfaceNumber(90f / halfVCount)}°";

            SurfacePivotText = $"Own center of the surface (rotation point): ({FormatSurfaceNumber(pivot.X)}; {FormatSurfaceNumber(pivot.Y)}; {FormatSurfaceNumber(pivot.Z)}) cm";

            SurfaceContourText = $"Pivot point of the contour on the grid: u = {FormatSurfaceNumber(mapping.Pivot.X)}°, v = {FormatSurfaceNumber(mapping.Pivot.Y)}°";
        }

        PointListTransformMessageParameter GetDrawingSurfaceMessage(PointListTransformMessageParameter parentMessage)
        {
            var identity = DrawingConstants.NON_TRANSFORM_MATRIX;

            var result = GetDrawingSpatialAxesMessage(parentMessage);

            result = GetDrawingCurveFigureMessage(result, _surfaceGrid,
                new DrawingColor(_drawingSettingsProvider.Settings.DrawingColor),
                identity,
                DrawingConstants.GRID_DRAWING_THICKNESS * 2);

            if (SurfaceContourVisible)
            {
                result = GetDrawingCurveFigureMessage(result, _surfaceContour,
                    new DrawingColor(DrawingConstants.SURFACE_CONTOUR_COLOR),
                    identity,
                    _drawingSettingsProvider.Settings.FigureDrawingThickness * 1.5f);

                if (!float.IsNaN(_surfaceContourPivotOnPlane.X))
                {
                    result = GetDrawingCurveFigureMessage(result, _surfaceContourPivotMarker,
                        new DrawingColor(DrawingConstants.SURFACE_CONTOUR_COLOR),
                        MatrixExtensions.CreateTranslationTransform(_surfaceContourPivotOnPlane.X, _surfaceContourPivotOnPlane.Y),
                        _drawingSettingsProvider.Settings.FigureDrawingThickness);
                }
            }

            return GetDrawingSpatialHelpersMessage(result);
        }

        void ResetSurfaceParameterValues()
        {
            SetSurfaceRadiusSilent(SurfaceRelatedConstants.DEFAULT_RADIUS_CENTIMETERS);
            SetSurfaceDepthCoefficientSilent(SurfaceRelatedConstants.DEFAULT_DEPTH_COEFFICIENT);
            SetSurfaceMeshStepUSilent(SurfaceRelatedConstants.DEFAULT_MESH_STEP_DEGREES);
            SetSurfaceMeshStepVSilent(SurfaceRelatedConstants.DEFAULT_MESH_STEP_DEGREES);
        }

        void ResetSurfaceTransformValues()
        {
            SetSurfaceTranslationXSilent(SurfaceRelatedConstants.DEFAULT_TRANSLATION_X_CENTIMETERS);
            SetSurfaceTranslationYSilent(SurfaceRelatedConstants.DEFAULT_TRANSLATION_Y_CENTIMETERS);
            SetSurfaceTranslationZSilent(SurfaceRelatedConstants.DEFAULT_TRANSLATION_Z_CENTIMETERS);
            SetSurfaceRotationXSilent(0);
            SetSurfaceRotationYSilent(0);
            SetSurfaceRotationZSilent(SurfaceRelatedConstants.DEFAULT_ROTATION_Z_DEGREES);
        }

        void ResetSurfaceContourTransformValues()
        {
            SetSurfaceContourShiftUSilent(0);
            SetSurfaceContourShiftVSilent(0);
            SetSurfaceContourAngleSilent(0);
            SetSurfaceContourScaleSilent(SurfaceRelatedConstants.DEFAULT_CONTOUR_SCALE_DEGREES_PER_CENTIMETER);
        }

        // the same three point perspective as in the lab 5, the centers are farther, so the surface is not stretched too much
        void ResetSurfaceProjectionValues()
        {
            SetProjectionCenterXSilent(SurfaceRelatedConstants.DEFAULT_CENTER_X_CENTIMETERS);
            SetProjectionCenterYSilent(SurfaceRelatedConstants.DEFAULT_CENTER_Y_CENTIMETERS);
            SetProjectionCenterZSilent(SurfaceRelatedConstants.DEFAULT_CENTER_Z_CENTIMETERS);
        }

        void ResetSurfaceAnimationValues()
        {
            SetSurfaceAnimationRadiusMinimumSilent(SurfaceRelatedConstants.DEFAULT_ANIMATION_RADIUS_MINIMUM);
            SetSurfaceAnimationRadiusMaximumSilent(SurfaceRelatedConstants.DEFAULT_ANIMATION_RADIUS_MAXIMUM);
            SetSurfaceAnimationDepthMinimumSilent(SurfaceRelatedConstants.DEFAULT_ANIMATION_DEPTH_MINIMUM);
            SetSurfaceAnimationDepthMaximumSilent(SurfaceRelatedConstants.DEFAULT_ANIMATION_DEPTH_MAXIMUM);
        }

        async Task StartSurfaceAnimationCoreAsync()
        {
            if (!_isSurfaceInitialized || IsSurfaceAnimationRunning)
            {
                return;
            }

            if (!_hasSurfaceAnimationStart)
            {
                _hasSurfaceAnimationStart = true;
                _surfaceAnimationStartRadius = SurfaceRadius;
                _surfaceAnimationStartDepth = SurfaceDepthCoefficient;
            }

            using (var animationCancellationSource = new CancellationTokenSource())
            {
                _surfaceAnimationCancellationSource = animationCancellationSource;

                IsSurfaceAnimationRunning = true;

                try
                {
                    await AnimateSurfaceAsync(animationCancellationSource.Token);
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
                    if (_surfaceAnimationCancellationSource == animationCancellationSource)
                    {
                        _surfaceAnimationCancellationSource = default;
                        IsSurfaceAnimationRunning = false;
                    }
                }
            }
        }

        // one step of a parameter between its bounds; at a bound the direction turns back
        static float GetAnimatedValue(float value, float bound1, float bound2, float step, ref bool isIncreasing)
        {
            var minimum = MathF.Min(bound1, bound2);
            var maximum = MathF.Max(bound1, bound2);

            if (value >= maximum)
            {
                isIncreasing = false;
            }
            else if (value <= minimum)
            {
                isIncreasing = true;
            }

            return (value + (isIncreasing ? step : -step)).Clamp(minimum, maximum);
        }

        // every frame the radius a and the depth coefficient k of the concave part move by a small step,
        // each of them goes back and forth between its own minimum and maximum
        async Task AnimateSurfaceAsync(CancellationToken token)
        {
            var userInterfaceConstants = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();
            var animationTimeoutMilliseconds = userInterfaceConstants.AnimationTimeoutMilliseconds;

            while (!token.IsCancellationRequested)
            {
                SetSurfaceRadiusSilent(GetAnimatedValue(SurfaceRadius, SurfaceAnimationRadiusMinimum, SurfaceAnimationRadiusMaximum,
                    SurfaceRelatedConstants.ANIMATION_RADIUS_STEP_CENTIMETERS, ref _isSurfaceAnimationIncreasing));
                SetSurfaceDepthCoefficientSilent(GetAnimatedValue(SurfaceDepthCoefficient, SurfaceAnimationDepthMinimum, SurfaceAnimationDepthMaximum,
                    SurfaceRelatedConstants.ANIMATION_DEPTH_STEP, ref _isSurfaceAnimationDepthIncreasing));

                UpdateSurfaceState();

                await RedrawAllAsync();

                await Task.Delay(animationTimeoutMilliseconds, token);
            }
        }

        async Task StopSurfaceAnimationAsync()
        {
            var animationCancellationSource = _surfaceAnimationCancellationSource;

            _surfaceAnimationCancellationSource = default;
            IsSurfaceAnimationRunning = false;

            await animationCancellationSource.TryCancelAsync(shouldDispose: false);
        }

        async Task ResetSurfaceAnimationCoreAsync()
        {
            await StopSurfaceAnimationAsync();

            if (_hasSurfaceAnimationStart)
            {
                SetSurfaceRadiusSilent(_surfaceAnimationStartRadius);
                SetSurfaceDepthCoefficientSilent(_surfaceAnimationStartDepth);
            }

            _hasSurfaceAnimationStart = false;
            _isSurfaceAnimationIncreasing = true;
            _isSurfaceAnimationDepthIncreasing = true;

            await RefreshSurfaceAsync();
        }
    }
}
