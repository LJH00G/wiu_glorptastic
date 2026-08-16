
using System.Runtime.CompilerServices;
using UnityEngine;


namespace Utility.Math
{
    public enum EASE : byte
    {
        LINEAR,
        IN_SIN,
        OUT_SIN,
        IN_OUT_SIN,
        IN_QUAD,
        OUT_QUAD,
        IN_OUT_QUAD,
        IN_ELASTIC,
        OUT_ELASTIC,
        IN_OUT_ELASTIC,
        IN_BACK,
        OUT_BACK,
        IN_OUT_BACK,
        IN_QUINT,
        OUT_QUINT,
        IN_OUT_QUINT,

        TOTAL_EASE
    }

    public static class Math_Ease
    {
        public interface IEaseCustomValue {}

        struct ElasticCustomValue : IEaseCustomValue
        {
            public float amplitude, period, shift, decay;
            public sbyte flip_val;
        }
        struct BackCustomValue : IEaseCustomValue
        {
            public float strength, strength_io;
        }

        /// <summary>
        /// factory for elastic ease custom value
        /// </summary>
        /// <param name="amplitude">force set to 1 if passed in value is less than 1</param>
        public static IEaseCustomValue CreateCustomElastic(float amplitude, float period, float decay, bool flip = false)
        {
            ElasticCustomValue customValue = new();
            customValue.amplitude = amplitude >= 1 ? amplitude : 1;
            customValue.period = period;
            customValue.shift = customValue.period / Math_C.PI_TWO * Mathf.Asin(1f / customValue.amplitude);
            customValue.decay = decay;
            customValue.flip_val = (sbyte)(flip ? -1 : 1);

            return customValue;
        }
        /// <summary>
        /// factory for back ease custom value
        /// </summary>
        public static IEaseCustomValue CreateCustomBack(float strength, float strength_io)
        {
            BackCustomValue customValue = new();
            customValue.strength = strength;
            customValue.strength_io = strength_io;

            return customValue;
        }


        public const float amplitude = 1f, period = 0.3f, shift = period / 4f, decay = 10f, strength = 1.70158f, strength_io = 2.59491f;
        public const sbyte flip_val = 1;

        /// <param name="t">value range from 0 - 1</param>
        /// <returns><paramref name="t"/> eased with ease equation</returns>
        public static float Ease(EASE ease, float t, IEaseCustomValue customValue = null)
        {
            switch (ease)
            {
                case EASE.LINEAR:
                    return t;

                case EASE.IN_SIN:
                    return 1 - Mathf.Cos(t * Math_C.PI_HALF);

                case EASE.OUT_SIN:
                    return Mathf.Sin(t * Math_C.PI_HALF);

                case EASE.IN_OUT_SIN:
                    return 0.5f * (1.0f - Mathf.Cos(t * Math_C.PI));

                case EASE.IN_QUAD:
                    return t * t;

                case EASE.OUT_QUAD:
                    return 1 - (1 - t) * (1 - t);

                case EASE.IN_OUT_QUAD:
                    if (t < 0.5f)
                        return 2 * t * t;
                    else
                        return 1 - ((-2 * t + 2) * (-2 * t + 2)) / 2;

                case EASE.IN_ELASTIC:
                    {
                        float a = amplitude, p = period, s = shift, d = decay;
                        sbyte flip_val = Math_Ease.flip_val;
                        ElasticCustomValueCheck(customValue, ref a, ref p, ref s, ref d, ref flip_val);

                        return -(a * Mathf.Pow(2f, d * (t - 1))) * flip_val * Mathf.Sin((t - 1 - s) * ((2f * Math_C.PI) / p));
                    }
                case EASE.OUT_ELASTIC:
                    {
                        float a = amplitude, p = period, s = shift, d = decay;
                        sbyte flip_val = Math_Ease.flip_val;
                        ElasticCustomValueCheck(customValue, ref a, ref p, ref s, ref d, ref flip_val);

                        return (a * Mathf.Pow(2f, -d * t) * flip_val * Mathf.Sin((t - s) * (2f * Math_C.PI) / p)) + 1f;
                    }
                case EASE.IN_OUT_ELASTIC:
                    {
                        float a = amplitude, p = period, s = shift, d = decay;
                        sbyte flip_val = Math_Ease.flip_val;
                        ElasticCustomValueCheck(customValue, ref a, ref p, ref s, ref d, ref flip_val);

                        if (t < 0.5f)
                            return -0.5f * (Mathf.Pow(2f, d * (2f * t - 1f)) * flip_val * Mathf.Sin(((2f * t - 1f) - s) * (2f * Math_C.PI) / p));
                        else
                            return 0.5f * (Mathf.Pow(2f, -d * (2f * t - 1f)) * flip_val * Mathf.Sin(((2f * t - 1f) - s) * (2f * Math_C.PI) / p)) + 1f;
                    }
                case EASE.IN_BACK:
                    {
                        float s = strength;
                        BackCustomValueCheck(customValue, ref s, false);

                        return t * t * ((s + 1) * t - s);
                    }
                case EASE.OUT_BACK:
                    {
                        float s = strength;
                        BackCustomValueCheck(customValue, ref s, false);

                        float adjustedT = t - 1.0f;
                        return adjustedT * adjustedT * ((s + 1) * adjustedT + s) + 1.0f;
                    }
                case EASE.IN_OUT_BACK:
                    {
                        float s = strength_io;
                        BackCustomValueCheck(customValue, ref s, true);

                        if (t < 0.5f)
                        {
                            float adjustedT = 2.0f * t;
                            return 0.5f * (adjustedT * adjustedT * ((s + 1) * adjustedT - s));
                        }
                        else
                        {
                            float adjustedT = 2.0f * t - 2.0f;
                            return 0.5f * (adjustedT * adjustedT * ((s + 1) * adjustedT + s) + 2.0f);
                        }
                    }
                case EASE.IN_QUINT:
                    return t * t * t * t * t;

                case EASE.OUT_QUINT:
                    {
                        float adjustedT = t - 1;
                        return adjustedT * adjustedT * adjustedT * adjustedT * adjustedT + 1;
                    }
                case EASE.IN_OUT_QUINT:
                    if (t < 0.5f)
                        return 2 * t * t * t * t * t;
                    else
                    {
                        float adjustedT = (2 * t) - 2;
                        return 0.5f * adjustedT * adjustedT * adjustedT * adjustedT * adjustedT + 1;
                    }
                default: return -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void ElasticCustomValueCheck(IEaseCustomValue customValue, ref float a, ref float p, ref float s, ref float d, ref sbyte flip_val)
            {
                Debug.Assert(customValue == null || customValue is ElasticCustomValue);
                if (customValue is ElasticCustomValue custom)
                {
                    a = custom.amplitude;
                    p = custom.period;
                    s = custom.shift;
                    d = custom.decay;
                    flip_val = custom.flip_val;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void BackCustomValueCheck(IEaseCustomValue customValue, ref float s, bool isIO)
            {
                Debug.Assert(customValue == null || customValue is BackCustomValue);
                if (customValue is BackCustomValue custom)
                {
                    if (isIO)
                        s = custom.strength_io;
                    else
                        s = custom.strength;
                }
            }
        }
    }
}