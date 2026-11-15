using System;
using System.Collections.Generic;
using System.Numerics;

namespace Modeling.Models.Drawing.Projection
{
    /// <summary>
    /// Faceted solid: a tower made of a regular N-gonal prism (the body) with an N-gonal pyramid on top (the roof).
    /// Coordinates are in centimeters around the own center of the solid (the middle of the body),
    /// which is the point the solid rotates around. The axis of the tower is Z.
    /// Vertices: 0…N−1 — bottom of the body, N…2N−1 — top of the body, 2N — apex of the roof.
    /// Edges: N bottom ones, N top ones, N vertical ones and N roof ones.
    /// </summary>
    public sealed class TowerPolyhedron
    {
        public const int MINIMUM_SIDES_COUNT = 3;
        public const int MAXIMUM_SIDES_COUNT = 12;

        public int SidesCount { get; }
        public float Radius { get; }
        public float Height { get; }
        public float RoofHeight { get; }

        public int VerticesCount => 2 * SidesCount + 1;
        public int EdgesCount => 4 * SidesCount;
        public int FacesCount => 2 * SidesCount + 1;

        public TowerPolyhedron(int sidesCount, float radius, float height, float roofHeight)
        {
            SidesCount = Math.Clamp(sidesCount, MINIMUM_SIDES_COUNT, MAXIMUM_SIDES_COUNT);
            Radius = radius;
            Height = height;
            RoofHeight = roofHeight;
        }

        public Vector3[] CreateVertices(float scale)
        {
            var result = new Vector3[VerticesCount];
            var halfHeight = Height / 2;

            for (var i = 0; i < SidesCount; i++)
            {
                var angle = 2 * MathF.PI * i / SidesCount;
                var x = Radius * MathF.Cos(angle);
                var y = Radius * MathF.Sin(angle);

                result[i] = new Vector3(x, y, -halfHeight) * scale;
                result[SidesCount + i] = new Vector3(x, y, halfHeight) * scale;
            }

            result[2 * SidesCount] = new Vector3(0, 0, halfHeight + RoofHeight) * scale;

            return result;
        }

        public IEnumerable<(int Start, int End)> GetEdges()
        {
            var apex = 2 * SidesCount;

            for (var i = 0; i < SidesCount; i++)
            {
                var next = (i + 1) % SidesCount;

                yield return (i, next);
                yield return (SidesCount + i, SidesCount + next);
                yield return (i, SidesCount + i);
                yield return (SidesCount + i, apex);
            }
        }
    }
}
