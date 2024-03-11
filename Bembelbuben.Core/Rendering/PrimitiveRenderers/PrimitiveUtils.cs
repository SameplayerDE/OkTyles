using System;
using Microsoft.Xna.Framework;

namespace Bembelbuben.Core.Rendering.PrimitiveRenderers
{
    public static class PrimitiveUtils
    {
        public static Vector2 Perpendicular(Vector2 vector)
        {
            var (x, y) = vector;
            return new Vector2(-y, x);
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }
    }
}