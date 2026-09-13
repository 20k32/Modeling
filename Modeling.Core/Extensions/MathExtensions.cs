using System;

namespace Modeling.Core.Extensions
{
    public static class MathExtensions
    {
        public static float DegreesToRadian(this float degrees) => degrees * MathF.PI / 180f;
        public static float RadianToDegrees(this float radians) => radians * 180f / MathF.PI;
    }
}
