
using System.Runtime.CompilerServices;

namespace Utility.Math
{
    public struct Math_I
    {
        /// <summary>wrap to <paramref name="minInclusive"/> if value goes above <paramref name="maxInclusive"/> after increment</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IncrementWrap(int value, int minInclusive, int maxInclusive)
        {
            return (value == maxInclusive) ?
                minInclusive :
                value + 1;
        }
        /// <summary>wrap to <paramref name="minInclusive"/> if value goes above <paramref name="maxInclusive"/> after increment</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint IncrementWrap(uint value, uint minInclusive, uint maxInclusive)
        {
            return (value == maxInclusive) ?
                minInclusive :
                value + 1;
        }

        /// <summary>wrap to <paramref name="maxInclusive"/> if value goes below <paramref name="minInclusive"/> after decrement</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DecrementWrap(int value, int minInclusive, int maxInclusive)
        {
            return (value == minInclusive) ?
                maxInclusive :
                value - 1;
        }

        /// <summary>wrap to <paramref name="maxInclusive"/> if value goes below <paramref name="minInclusive"/> after decrement</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DecrementWrap(uint value, uint minInclusive, uint maxInclusive)
        {
            return (value == minInclusive) ?
                maxInclusive :
                value - 1;
        }
    }
}