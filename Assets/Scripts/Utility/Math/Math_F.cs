

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utility.Math
{
    public struct Math_F
    {
        /// <summary>compare if the floats are equal with your epsilon</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(float a, float b, float epsilon)
        {
            return Mathf.Abs(a - b) < epsilon;
        }
        /// <summary>compare if the floats are equal</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(float a, float b)
        {
            return Mathf.Abs(a - b) < Math_C.F_EPSILON;
        }
        /// <summary>compare if the floats are equal, uses epsilon of 3 decimal places</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal_3d(float a, float b)
        {
            return Mathf.Abs(a - b) < Math_C.F_EPSILON_3D;
        }

        /// <summary>wrap the value to the other extreme</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Wrap(float value, float min, float max)
        {
            if (value < min)
                return value + (max - min);
            if (value > max)
                return value - (max - min);
            return value;
        }
    }
}