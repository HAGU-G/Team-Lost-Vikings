using System;
using UnityEngine;

[Serializable]
public struct Ellipse
{
    public float a;
    public float b;

    public Vector2 position;

    public float CollisionDepthWith(Ellipse other)
    {
        return CollisionDepth(this, other);
    }

    public bool IsCollidedWith(Ellipse other)
    {
        return IsCollided(this, other);
    }

    public static Vector2 GetPoint(float a, float b, Vector2 zeroPos, float radian)
    {
        return new(zeroPos.x + a * Mathf.Cos(radian), zeroPos.y + b * Mathf.Sin(radian));
    }

    public static Vector2 GetPoint(Ellipse ellipse, float radian)
    {
        return GetPoint(ellipse.a, ellipse.b, ellipse.position, radian);
    }

    public static bool IsCollided(Ellipse ellipse1, Ellipse ellipse2)
    {
        return CollisionDepth(ellipse1, ellipse2) > 0f;
    }

    public static float CollisionDepth(Ellipse ellipse1, Ellipse ellipse2, GameObject point = null)
    {
        var direc = (ellipse2.position - ellipse1.position);
        var angle = Mathf.Atan2(direc.y, direc.x);
        if (point != null)
            point.transform.position = GetPoint(ellipse1, angle);
        var radius1 = (GetPoint(ellipse1, angle + Mathf.PI) - ellipse1.position).magnitude;
        var radius2 = (GetPoint(ellipse2, angle) - ellipse2.position).magnitude;

        return (radius1 + radius2) - direc.magnitude;
    }
}