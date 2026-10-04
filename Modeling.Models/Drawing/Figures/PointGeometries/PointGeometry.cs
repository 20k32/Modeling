using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Models.Drawing.Figures.PointGeometries;

abstract class PointGeometry(float minimumAcceptableDistance = Constants.MINIMUM_ACCEPTABLE_DISTANCE,
    float maximumAcceptableDistance = Constants.MAXIMUM_ACCEPATABLE_DISTANCE) : ObservableObject, IPointGeometry
{
    readonly float _minimumAcceptableDistance = minimumAcceptableDistance;
    readonly float _maximumAcceptableDistance = maximumAcceptableDistance;

    float _distance;

    PointSingle _defaultCenterPoint;
    PointSingle _centerPoint;
    RectangleSingle _bounds;

    public IPointHashSetCollection DefaultPoints { get; private init; } = Ioc.Default.GetRequiredService<IPointHashSetCollection>();
    public IPointHashSetCollection Points { get; private init; } = Ioc.Default.GetRequiredService<IPointHashSetCollection>();

    public SegmentDimensionParameter SegmentDimensionParameter { get; private set; }

    public PointSingle DefaultCenterPoint => _defaultCenterPoint;
    public PointSingle CenterPoint => _centerPoint;
    public RectangleSingle Bounds => _bounds;

    public abstract GeometryType GeometryType { get; protected set; }
    public abstract DimensionType DimensionType { get; protected set; }

    public event ActionEventHandler PointGeometryPropertyChanged;

    protected float Distance
    {
        get => _distance;
        set
        {
            var newValue = MathFloatExtensions.Clamp(value, _minimumAcceptableDistance, _maximumAcceptableDistance);

            if (_distance != newValue)
            {
                _distance = newValue;

                InvokePointGeometryPropertyChanged();
            }
        }
    }

    public SegmentDimensionParameter SegmentDimension => throw new NotImplementedException();

    public void AddPoint(PointSingle point) => Points.Add(point);
    public void AddPointsRange(IEnumerable<PointSingle> points)
    {
        foreach (var point in points)
        {
            AddPoint(point);
        }
    }

    public void CalculateBounds()
    {
        var left = float.PositiveInfinity;
        var top = float.PositiveInfinity;
        var right = float.NegativeInfinity;
        var bottom = float.NegativeInfinity;

        foreach (var point in Points)
        {
            left = MathF.Min(left, point.X);
            top = MathF.Min(top, point.Y);
            right = MathF.Max(right, point.X);
            bottom = MathF.Max(bottom, point.Y);
        }

        _bounds = new(top, left, right, bottom);

        CalculateCenterPoint();

        _distance = CalculateDistance();
    }

    public void CalculateCenterPoint()
        => _centerPoint = CalculateCenterPointCore();

    public void CommitPropertyChanges() => InvokePointGeometryPropertyChanged();

    public bool ContainsPoint(PointSingle point) => Points.Contains(point);

    public void SetDefaultProperties()
    {
        DefaultPoints.AddRange(Points);
        _defaultCenterPoint = CenterPoint;
    }

    public void ClearPoints() => Points.Clear();


    protected void InvokePointGeometryPropertyChanged() => PointGeometryPropertyChanged?.Invoke();

    protected virtual float CalculateDistance()
        => MathF.Sqrt(_bounds.Width * _bounds.Width + _bounds.Height * _bounds.Height);

    protected virtual PointSingle CalculateCenterPointCore()
        => new PointSingle((Bounds.Left + Bounds.Right) / 2, (Bounds.Top + Bounds.Bottom) / 2);

    public void SetSegmentDimension(SegmentDimensionParameter segmentDimension)
        => SegmentDimensionParameter = segmentDimension;
}
