using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using Modeling.Models.Enums;
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
    public IAdjacentPointGeometryCollection AdjacentGeometries { get; private init; } = Ioc.Default.GetRequiredService<IAdjacentPointGeometryCollection>();

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

    public void AddAdjacentGeometry(IPointGeometry geometry, AdjacentType adjacentType)
    {
        var firstAdjacentGeometry = Ioc.Default.GetRequiredService<IAdjacentPointGeometry>();

        firstAdjacentGeometry.AdjacentType = adjacentType;
        firstAdjacentGeometry.Geometry = geometry;

        AdjacentGeometries.AddUnique(firstAdjacentGeometry);

        var secondAdjacentGeometry = Ioc.Default.GetRequiredService<IAdjacentPointGeometry>();

        secondAdjacentGeometry.AdjacentType = adjacentType.Invert();
        secondAdjacentGeometry.Geometry = geometry;

        geometry.AdjacentGeometries.AddUnique(secondAdjacentGeometry);
    }

    public void RemoveAdjacentGeometry(IPointGeometry geometry)
    {
        var geometriesToRemove = (ICollection<IAdjacentPointGeometry>)[.. AdjacentGeometries.Where(existing => existing.Geometry == geometry)];

        AdjacentGeometries.RemoveRange(geometriesToRemove);

        geometriesToRemove = [.. geometry.AdjacentGeometries.Where(existing => existing.Geometry == geometry)];
        geometry.AdjacentGeometries.RemoveRange(geometriesToRemove);
    }

    public void ClearAdjacentGeometries()
    {
        foreach (var adjacentGeometry in AdjacentGeometries)
        {
            if (adjacentGeometry.Geometry.AdjacentGeometries.Select(existing => existing.Geometry).Contains(this))
            {
                var geometriesToRemove = (ICollection<IAdjacentPointGeometry>)[.. adjacentGeometry.Geometry.AdjacentGeometries.Where(existing => existing.Geometry == adjacentGeometry)];

                adjacentGeometry.Geometry.AdjacentGeometries.RemoveRange(geometriesToRemove);
            }
        }

        AdjacentGeometries.Clear();
    }

    public void AddAdjacentGeometriesRange(IEnumerable<IPointGeometry> geometries, AdjacentType adjacentType)
    {
        foreach (var geometry in geometries)
        {
            var adjacentGeometry = Ioc.Default.GetRequiredService<IAdjacentPointGeometry>();
            adjacentGeometry.AdjacentType = adjacentType;
            adjacentGeometry.Geometry = geometry;

            AdjacentGeometries.Add(adjacentGeometry);
        }
    }
}
