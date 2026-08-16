
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utility.Math
{
    public struct Math_Vec
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            if (value.x < min.x)
                value.x = min.x;
            if (value.y < min.y)
                value.y = min.y;

            if (value.x > max.x)
                value.x = max.x;
            if (value.y > max.y)
                value.y = max.y;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            if (value.x < min.x)
                value.x = min.x;
            if (value.y < min.y)
                value.y = min.y;
            if (value.z < min.z)
                value.z = min.z;

            if (value.x > max.x)
                value.x = max.x;
            if (value.y > max.y)
                value.y = max.y;
            if (value.z > max.z)
                value.z = max.z;

            return value;
        }
    }
}