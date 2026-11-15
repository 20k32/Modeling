using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Drawing.Projection;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using Modeling.Models.UserInterface;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    // Lab 5: faceted solid (tower) in the space, three point perspective on the plane x = 0.
    // The solid lives in centimeters, the projection plane is drawn with y to the right and z up, its origin is the canvas center.
    public sealed partial class DrawingViewModel : BaseViewModel
    {
        readonly IFigure _polyhedronEdges = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _spatialAxisX = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _spatialAxisY = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _spatialAxisZ = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _polyhedronOwnAxisX = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _polyhedronOwnAxisY = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _polyhedronOwnAxisZ = Ioc.Default.GetRequiredService<IFigure>();
        readonly IFigure _polyhedronPointMarker = Ioc.Default.GetRequiredService<IFigure>();

        readonly List<IPointGeometry> _polyhedronEdgeGeometries = [];

        PointSingle _polyhedronCenter;
        float _polyhedronPixelsPerCentimeter;

        bool _isPolyhedronInitialized;
        bool _isPolyhedronAnimationHeightIncreasing;

        PointSingle _polyhedronPivotOnPlane;
        PointSingle[] _vanishingPointsOnPlane = [];

        // values restored by the animation reset
        float _polyhedronAnimationStartHeight;
        float _polyhedronAnimationStartRotationX;
        float _polyhedronAnimationStartRotationY;
        float _polyhedronAnimationStartRotationZ;
        bool _hasPolyhedronAnimationStart;

        CancellationTokenSource _polyhedronAnimationCancellationSource;

        void InitializePolyhedron(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            _polyhedronCenter = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);
            _polyhedronPixelsPerCentimeter = pixelsPerCentimeter;
            _isPolyhedronAnimationHeightIncreasing = true;
            _hasPolyhedronAnimationStart = false;

            ResetPolyhedronParameterValues();
            ResetPolyhedronTransformValues();
            ResetProjectionValues();
            ResetPolyhedronAnimationValues();

            InitializePolyhedronFigures(pixelsPerCentimeter);

            _isPolyhedronInitialized = true;

            UpdatePolyhedronState();
        }

        void ClearPolyhedron()
        {
            _isPolyhedronInitialized = false;

            _ = StopPolyhedronAnimationAsync();

            _polyhedronEdges.Clear();
            _polyhedronEdgeGeometries.Clear();

            foreach (var figure in GetPolyhedronLineFigures())
            {
                figure.Clear();
            }

            _polyhedronPointMarker.Clear();
        }

        IEnumerable<IFigure> GetPolyhedronLineFigures()
        {
            yield return _spatialAxisX;
            yield return _spatialAxisY;
            yield return _spatialAxisZ;
            yield return _polyhedronOwnAxisX;
            yield return _polyhedronOwnAxisY;
            yield return _polyhedronOwnAxisZ;
        }

        void InitializePolyhedronFigures(float pixelsPerCentimeter)
        {
            var geometryFactory = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>();

            foreach (var figure in GetPolyhedronLineFigures())
            {
                figure.AddSegment(geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None), pixelsPerCentimeter);
            }

            var markerCenter = DrawingConstants.DEFAULT_POINT;

            _polyhedronPointMarker.AddSegment(
                markerCenter.GetCirclePoints(PolyhedronRelatedConstants.POINT_MARKER_RADIUS_PIXELS).ToCirclePointGeometry(markerCenter),
                pixelsPerCentimeter);
            _polyhedronPointMarker.CalculatePropertiesFromSegments();
        }

        // one line geometry per edge, rebuilt only when the amount of the tower sides changes
        void EnsurePolyhedronEdgeGeometries(int edgesCount)
        {
            if (_polyhedronEdgeGeometries.Count == edgesCount)
            {
                return;
            }

            _polyhedronEdges.Clear();
            _polyhedronEdgeGeometries.Clear();

            var geometryFactory = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>();

            for (var i = 0; i < edgesCount; i++)
            {
                var geometry = geometryFactory.CreateLinePointGeometry(SegmentDimensionParameter.None);

                _polyhedronEdgeGeometries.Add(geometry);
                _polyhedronEdges.AddSegment(geometry, _polyhedronPixelsPerCentimeter);
            }
        }

        async Task RefreshPolyhedronAsync()
        {
            if (_isPolyhedronInitialized)
            {
                UpdatePolyhedronState();

                await RedrawAllAsync();
            }
        }

        async Task RefreshPolyhedronWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(RefreshPolyhedronAsync);
        }

        PointSingle ToProjectionCanvasPoint(PointSingle planePoint)
            => new(
                _polyhedronCenter.X + planePoint.X * _polyhedronPixelsPerCentimeter,
                _polyhedronCenter.Y - planePoint.Y * _polyhedronPixelsPerCentimeter);

        Matrix4x4 GetPolyhedronWorldTransform()
            => SpatialTransforms.Create(
                PolyhedronRotationX,
                PolyhedronRotationY,
                PolyhedronRotationZ,
                new Vector3(PolyhedronTranslationX, PolyhedronTranslationY, PolyhedronTranslationZ));

        Matrix4x4 GetProjectionMatrix()
            => ThreePointPerspective.CreateMatrix(ProjectionCenterX, ProjectionCenterY, ProjectionCenterZ);

        // an edge is drawn only when both of its ends are in front of the centers of projection
        void SetProjectedLine(IPointGeometry geometry, Vector3 start, Vector3 end, Matrix4x4 projection)
        {
            geometry.ClearPoints();

            if (ThreePointPerspective.TryProject(start, projection, out var projectedStart)
                && ThreePointPerspective.TryProject(end, projection, out var projectedEnd))
            {
                geometry.AddPoint(ToProjectionCanvasPoint(projectedStart));
                geometry.AddPoint(ToProjectionCanvasPoint(projectedEnd));
            }

            geometry.CalculateBounds();
        }

        static IPointGeometry GetLineGeometry(IFigure figure) => figure.Segments[0];

        // the positive half of an axis, shortened so that it stays in front of the center of projection on this axis
        static float GetVisibleAxisLength(float centerDistance)
            => centerDistance > 0
                ? MathF.Min(PolyhedronRelatedConstants.SPATIAL_AXIS_LENGTH_CENTIMETERS, PolyhedronRelatedConstants.SPATIAL_AXIS_CENTER_RATIO * centerDistance)
                : PolyhedronRelatedConstants.SPATIAL_AXIS_LENGTH_CENTIMETERS;

        TowerPolyhedron CreateTower()
            => new((int)PolyhedronSidesCount, PolyhedronRadius, PolyhedronHeight, PolyhedronRoofHeight);

        // every vertex goes through the euclidean transform of the solid and then through the projection matrix
        void UpdatePolyhedronState()
        {
            var tower = CreateTower();
            var world = GetPolyhedronWorldTransform();
            var projection = GetProjectionMatrix();

            EnsurePolyhedronEdgeGeometries(tower.EdgesCount);

            var vertices = tower.CreateVertices(PolyhedronScale);

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Transform(vertices[i], world);
            }

            var edgeIndex = 0;

            foreach (var (start, end) in tower.GetEdges())
            {
                SetProjectedLine(_polyhedronEdgeGeometries[edgeIndex++], vertices[start], vertices[end], projection);
            }

            SetProjectedLine(GetLineGeometry(_spatialAxisX), Vector3.Zero, new Vector3(GetVisibleAxisLength(ProjectionCenterX), 0, 0), projection);
            SetProjectedLine(GetLineGeometry(_spatialAxisY), Vector3.Zero, new Vector3(0, GetVisibleAxisLength(ProjectionCenterY), 0), projection);
            SetProjectedLine(GetLineGeometry(_spatialAxisZ), Vector3.Zero, new Vector3(0, 0, GetVisibleAxisLength(ProjectionCenterZ)), projection);

            // own axes of the solid go through its center and turn together with it
            var ownAxisLength = PolyhedronRelatedConstants.OWN_AXIS_LENGTH_RATIO * MathF.Max(tower.Radius, tower.Height / 2 + tower.RoofHeight) * PolyhedronScale;

            SetProjectedLine(GetLineGeometry(_polyhedronOwnAxisX), Vector3.Transform(-Vector3.UnitX * ownAxisLength, world), Vector3.Transform(Vector3.UnitX * ownAxisLength, world), projection);
            SetProjectedLine(GetLineGeometry(_polyhedronOwnAxisY), Vector3.Transform(-Vector3.UnitY * ownAxisLength, world), Vector3.Transform(Vector3.UnitY * ownAxisLength, world), projection);
            SetProjectedLine(GetLineGeometry(_polyhedronOwnAxisZ), Vector3.Transform(-Vector3.UnitZ * ownAxisLength, world), Vector3.Transform(Vector3.UnitZ * ownAxisLength, world), projection);

            var pivot = Vector3.Transform(Vector3.Zero, world);

            _polyhedronPivotOnPlane = ThreePointPerspective.TryProject(pivot, projection, out var projectedPivot)
                ? ToProjectionCanvasPoint(projectedPivot)
                : DrawingConstants.BREAK_POINT;

            _vanishingPointsOnPlane =
            [
                ToProjectionCanvasPoint(ThreePointPerspective.GetVanishingPointX()),
                ToProjectionCanvasPoint(ThreePointPerspective.GetVanishingPointY(ProjectionCenterY)),
                ToProjectionCanvasPoint(ThreePointPerspective.GetVanishingPointZ(ProjectionCenterZ)),
            ];

            UpdatePolyhedronInformation(tower, projection, pivot);
        }

        static string FormatPolyhedronNumber(float value) => value.ToString(PolyhedronRelatedConstants.NUMBER_FORMAT);

        void UpdatePolyhedronInformation(TowerPolyhedron tower, Matrix4x4 projection, Vector3 pivot)
        {
            PolyhedronInfoText = $"Tower: {tower.SidesCount}-gonal prism with a pyramid roof, vertices: {tower.VerticesCount}, edges: {tower.EdgesCount}, faces: {tower.FacesCount}";

            PolyhedronPivotText = $"Own center (rotation point): ({FormatPolyhedronNumber(pivot.X)}; {FormatPolyhedronNumber(pivot.Y)}; {FormatPolyhedronNumber(pivot.Z)}) cm";

            ProjectionMatrixText = "M = | " + FormatMatrixRow(projection.M11, projection.M12, projection.M13, projection.M14) + " |\n"
                + "      | " + FormatMatrixRow(projection.M21, projection.M22, projection.M23, projection.M24) + " |\n"
                + "      | " + FormatMatrixRow(projection.M31, projection.M32, projection.M33, projection.M34) + " |\n"
                + "      | " + FormatMatrixRow(projection.M41, projection.M42, projection.M43, projection.M44) + " |";

            var vanishingPointY = ThreePointPerspective.GetVanishingPointY(ProjectionCenterY);
            var vanishingPointZ = ThreePointPerspective.GetVanishingPointZ(ProjectionCenterZ);

            VanishingPointsText = "Vanishing points on the plane x = 0 (y; z): X (0; 0), "
                + $"Y ({FormatPolyhedronNumber(vanishingPointY.X)}; 0), Z (0; {FormatPolyhedronNumber(vanishingPointZ.Y)}) cm";
        }

        static string FormatMatrixRow(float m1, float m2, float m3, float m4)
            => $"{FormatPolyhedronNumber(m1),7}  {FormatPolyhedronNumber(m2),7}  {FormatPolyhedronNumber(m3),7}  {FormatPolyhedronNumber(m4),7}";

        PointListTransformMessageParameter GetDrawingPolyhedronMessage(PointListTransformMessageParameter parentMessage)
        {
            var identity = DrawingConstants.NON_TRANSFORM_MATRIX;
            var axisThickness = _drawingSettingsProvider.Settings.AxisThickness;
            var ownAxisThickness = DrawingConstants.GRID_DRAWING_THICKNESS * 2;

            var result = GetDrawingCurveFigureMessage(parentMessage, _spatialAxisX,
                new DrawingColor(DrawingConstants.X_AXIS_COLOR), identity, axisThickness);
            result = GetDrawingCurveFigureMessage(result, _spatialAxisY, new DrawingColor(DrawingConstants.Y_AXIS_COLOR), identity);
            result = GetDrawingCurveFigureMessage(result, _spatialAxisZ, new DrawingColor(DrawingConstants.Z_AXIS_COLOR), identity);

            result = GetDrawingCurveFigureMessage(result, _polyhedronEdges,
                new DrawingColor(_drawingSettingsProvider.Settings.DrawingColor),
                identity,
                _drawingSettingsProvider.Settings.FigureDrawingThickness);

            if (PolyhedronOwnAxesVisible)
            {
                result = GetDrawingCurveFigureMessage(result, _polyhedronOwnAxisX, new DrawingColor(DrawingConstants.X_AXIS_COLOR), identity, ownAxisThickness);
                result = GetDrawingCurveFigureMessage(result, _polyhedronOwnAxisY, new DrawingColor(DrawingConstants.Y_AXIS_COLOR), identity);
                result = GetDrawingCurveFigureMessage(result, _polyhedronOwnAxisZ, new DrawingColor(DrawingConstants.Z_AXIS_COLOR), identity);

                if (!float.IsNaN(_polyhedronPivotOnPlane.X))
                {
                    result = GetDrawingCurveFigureMessage(result, _polyhedronPointMarker,
                        new DrawingColor(DrawingConstants.CURVE_ROTATION_POINT_COLOR),
                        MatrixExtensions.CreateTranslationTransform(_polyhedronPivotOnPlane.X, _polyhedronPivotOnPlane.Y),
                        shouldFillGeometry: true);
                }
            }

            if (VanishingPointsVisible)
            {
                foreach (var vanishingPoint in _vanishingPointsOnPlane)
                {
                    result = GetDrawingCurveFigureMessage(result, _polyhedronPointMarker,
                        new DrawingColor(DrawingConstants.VANISHING_POINT_COLOR),
                        MatrixExtensions.CreateTranslationTransform(vanishingPoint.X, vanishingPoint.Y),
                        shouldFillGeometry: true);
                }
            }

            return result;
        }

        void ResetPolyhedronParameterValues()
        {
            SetPolyhedronSidesCountSilent(PolyhedronRelatedConstants.DEFAULT_SIDES_COUNT);
            SetPolyhedronRadiusSilent(PolyhedronRelatedConstants.DEFAULT_RADIUS_CENTIMETERS);
            SetPolyhedronHeightSilent(PolyhedronRelatedConstants.DEFAULT_HEIGHT_CENTIMETERS);
            SetPolyhedronRoofHeightSilent(PolyhedronRelatedConstants.DEFAULT_ROOF_HEIGHT_CENTIMETERS);
            SetPolyhedronScaleSilent(PolyhedronRelatedConstants.DEFAULT_SCALE);
        }

        void ResetPolyhedronTransformValues()
        {
            SetPolyhedronTranslationXSilent(PolyhedronRelatedConstants.DEFAULT_TRANSLATION_X_CENTIMETERS);
            SetPolyhedronTranslationYSilent(PolyhedronRelatedConstants.DEFAULT_TRANSLATION_Y_CENTIMETERS);
            SetPolyhedronTranslationZSilent(PolyhedronRelatedConstants.DEFAULT_TRANSLATION_Z_CENTIMETERS);
            SetPolyhedronRotationXSilent(0);
            SetPolyhedronRotationYSilent(0);
            SetPolyhedronRotationZSilent(PolyhedronRelatedConstants.DEFAULT_ROTATION_Z_DEGREES);
        }

        void ResetProjectionValues()
        {
            SetProjectionCenterXSilent(PolyhedronRelatedConstants.DEFAULT_CENTER_X_CENTIMETERS);
            SetProjectionCenterYSilent(PolyhedronRelatedConstants.DEFAULT_CENTER_Y_CENTIMETERS);
            SetProjectionCenterZSilent(PolyhedronRelatedConstants.DEFAULT_CENTER_Z_CENTIMETERS);
        }

        void ResetPolyhedronAnimationValues()
        {
            SetPolyhedronAnimationHeightMinimumSilent(PolyhedronRelatedConstants.DEFAULT_ANIMATION_HEIGHT_MINIMUM);
            SetPolyhedronAnimationHeightMaximumSilent(PolyhedronRelatedConstants.DEFAULT_ANIMATION_HEIGHT_MAXIMUM);
            SetPolyhedronAnimationRotationStepXSilent(0);
            SetPolyhedronAnimationRotationStepYSilent(0);
            SetPolyhedronAnimationRotationStepZSilent(PolyhedronRelatedConstants.DEFAULT_ANIMATION_ROTATION_STEP_DEGREES);
        }

        static float WrapAngle(float degrees)
        {
            var result = degrees % 360;

            if (result > 180)
            {
                result -= 360;
            }
            else if (result < -180)
            {
                result += 360;
            }

            return result;
        }

        async Task StartPolyhedronAnimationCoreAsync()
        {
            if (!_isPolyhedronInitialized || IsPolyhedronAnimationRunning)
            {
                return;
            }

            if (!_hasPolyhedronAnimationStart)
            {
                _hasPolyhedronAnimationStart = true;
                _polyhedronAnimationStartHeight = PolyhedronHeight;
                _polyhedronAnimationStartRotationX = PolyhedronRotationX;
                _polyhedronAnimationStartRotationY = PolyhedronRotationY;
                _polyhedronAnimationStartRotationZ = PolyhedronRotationZ;
            }

            using (var animationCancellationSource = new CancellationTokenSource())
            {
                _polyhedronAnimationCancellationSource = animationCancellationSource;

                IsPolyhedronAnimationRunning = true;

                try
                {
                    await AnimatePolyhedronAsync(animationCancellationSource.Token);
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
                    if (_polyhedronAnimationCancellationSource == animationCancellationSource)
                    {
                        _polyhedronAnimationCancellationSource = default;
                        IsPolyhedronAnimationRunning = false;
                    }
                }
            }
        }

        // every frame the height moves between its minimum and maximum, and the solid turns by the given offsets around its own axes
        async Task AnimatePolyhedronAsync(CancellationToken token)
        {
            var userInterfaceConstants = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();
            var animationTimeoutMilliseconds = userInterfaceConstants.AnimationTimeoutMilliseconds;

            while (!token.IsCancellationRequested)
            {
                var minimum = MathF.Min(PolyhedronAnimationHeightMinimum, PolyhedronAnimationHeightMaximum);
                var maximum = MathF.Max(PolyhedronAnimationHeightMinimum, PolyhedronAnimationHeightMaximum);

                if (PolyhedronHeight >= maximum)
                {
                    _isPolyhedronAnimationHeightIncreasing = false;
                }
                else if (PolyhedronHeight <= minimum)
                {
                    _isPolyhedronAnimationHeightIncreasing = true;
                }

                var heightStep = _isPolyhedronAnimationHeightIncreasing
                    ? PolyhedronRelatedConstants.ANIMATION_HEIGHT_STEP_CENTIMETERS
                    : -PolyhedronRelatedConstants.ANIMATION_HEIGHT_STEP_CENTIMETERS;

                SetPolyhedronHeightSilent((PolyhedronHeight + heightStep).Clamp(minimum, maximum));
                SetPolyhedronRotationXSilent(WrapAngle(PolyhedronRotationX + PolyhedronAnimationRotationStepX));
                SetPolyhedronRotationYSilent(WrapAngle(PolyhedronRotationY + PolyhedronAnimationRotationStepY));
                SetPolyhedronRotationZSilent(WrapAngle(PolyhedronRotationZ + PolyhedronAnimationRotationStepZ));

                UpdatePolyhedronState();

                await RedrawAllAsync();

                await Task.Delay(animationTimeoutMilliseconds, token);
            }
        }

        async Task StopPolyhedronAnimationAsync()
        {
            var animationCancellationSource = _polyhedronAnimationCancellationSource;

            _polyhedronAnimationCancellationSource = default;
            IsPolyhedronAnimationRunning = false;

            await animationCancellationSource.TryCancelAsync(shouldDispose: false);
        }

        async Task ResetPolyhedronAnimationCoreAsync()
        {
            await StopPolyhedronAnimationAsync();

            if (_hasPolyhedronAnimationStart)
            {
                SetPolyhedronHeightSilent(_polyhedronAnimationStartHeight);
                SetPolyhedronRotationXSilent(_polyhedronAnimationStartRotationX);
                SetPolyhedronRotationYSilent(_polyhedronAnimationStartRotationY);
                SetPolyhedronRotationZSilent(_polyhedronAnimationStartRotationZ);
            }

            _hasPolyhedronAnimationStart = false;
            _isPolyhedronAnimationHeightIncreasing = true;

            await RefreshPolyhedronAsync();
        }
    }
}
