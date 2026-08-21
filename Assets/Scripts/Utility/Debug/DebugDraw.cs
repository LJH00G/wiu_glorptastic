using UnityEngine;


public class DebugDraw
{

    public enum SIDES : int {
        S8 = 8,
        S12 = 12,
        S16 = 16,
        S24 = 24,
        S32 = 32
    }

    static float GetAngleDelta(SIDES sides)
    {
        switch (sides)
        {
            default:
            case SIDES.S8: return 45f;
            case SIDES.S12: return 30f;
            case SIDES.S16: return 22.5f;
            case SIDES.S24: return 15f;
            case SIDES.S32: return 11.25f;
        }
    }

    static public bool Enabled { get; set; } = false;
    static public Color Color { get; set; } = Color.blue;
    static public float Duration { get; set; } = 0.1f;

    static public void Box(Vector2 center, Vector2 size)
    {
        if (!Enabled)
            return;

        Vector2 half = size * 0.5f;

        Vector2 a = center + new Vector2(-half.x, -half.y);
        Vector2 b = center + new Vector2(-half.x, half.y);
        Vector2 c = center + new Vector2(half.x, half.y);
        Vector2 d = center + new Vector2(half.x, -half.y);

        Debug.DrawLine(a, b, Color, Duration);
        Debug.DrawLine(b, c, Color, Duration);
        Debug.DrawLine(c, d, Color, Duration);
        Debug.DrawLine(d, a, Color, Duration);
    }


    static public void Circle(Vector2 center, float radius, SIDES sides = SIDES.S16)
    {
        if (!Enabled)
            return;

        int sides_int = (int)sides;
        float angleDelta = GetAngleDelta(sides);

        Vector2 prevPoint = center + Vector2.right * radius;

        for (int i = 1; i <= sides_int; i++)
        {
            float angle = angleDelta * i * Mathf.Deg2Rad;

            Vector2 currentPoint = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            Debug.DrawLine(prevPoint, currentPoint, Color, Duration);

            prevPoint = currentPoint;
        }
    }
}
