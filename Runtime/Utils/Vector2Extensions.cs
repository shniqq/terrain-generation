using UnityEngine;

namespace TerrainGeneration.Utils
{
    internal static class Vector2Extensions
    {
        internal static bool IsValueWithinXY(this Vector2 vector2, float value)
        {
            return value >= vector2.x && value <= vector2.y;
        }
    }
}