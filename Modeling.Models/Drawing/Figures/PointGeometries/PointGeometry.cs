using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures.PointGeometries;

abstract class PointGeometry(float minimumAcceptableDistance = Constants.MINIMUM_ACCEPTABLE_DISTANCE,
    float maximumAcceptableDistance = Constants.MAXIMUM_ACCEPATABLE_DISTANCE) : ObservableObject, IPointGeometry
{
    public abstract GeometryType GeometryType { get; protected set; }
    public abstract DimensionType DimensionType { get; protected set; }

    RectangleSingle _bounds;
    PointSingle _centerPoint;
    float _minimumAcceptableDistance = minimumAcceptableDistance;
    float _maximumAcceptableDistance = maximumAcceptableDistance;

    public PointSingle CenterPoint => _centerPoint;

    public RectangleSingle Bounds => _bounds;
    public HashSet<PointSingle> Points { get; } = [];

    public event ActionEventHandler PointGeometryPropertyChanged;

    public bool TryAddPoint(PointSingle point) => Points.Add(point);
    public void AddPointsRange(IEnumerable<PointSingle> points)
    {
        foreach (var point in points)
        {
            _ = TryAddPoint(point);
        }
    }

    public void CalculateBounds()
    {
        var left = float.MaxValue;
        var top = float.MaxValue;
        var right = float.MinValue;
        var bottom = float.MinValue;

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
        => _centerPoint = new PointSingle((Bounds.Left + Bounds.Right)  / 2, (Bounds.Top + Bounds.Bottom) / 2);

    public void Commit() => InvokePointGeometryPropertyChanged();

    protected void InvokePointGeometryPropertyChanged() => PointGeometryPropertyChanged?.Invoke();
    private float _distance;
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

    protected virtual float CalculateDistance()
        => MathF.Sqrt(_bounds.Width * _bounds.Width + _bounds.Height * _bounds.Height);
}
