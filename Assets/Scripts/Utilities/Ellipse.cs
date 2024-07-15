using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Ellipse
{
    private static GameSetting setting = GameSetting.Instance;

    public Ellipse(float majorAxis, float minorAxis, Vector3 pos)
    {
        SetAxies(majorAxis, minorAxis);
        position = pos;
    }
    public Ellipse(float majorAxis, float minorAxis) : this(majorAxis, minorAxis, Vector2.zero) { }
    public Ellipse(float majorAxis, Vector2 pos) : this(majorAxis, majorAxis * setting.ellipseRatio, pos) { }
    public Ellipse(float majorAxis) : this(majorAxis, Vector2.zero) { }

    public float a; //major
    public float b; //minor
    public float maxAxis;

    public Vector2 position;

    public float CollisionDepthWith(Ellipse other)
    {
        return CollisionDepth(this, other);
    }

    public bool IsCollidedWith(Ellipse other)
    {
        return IsCollided(this, other);
    }

    public void SetAxies(float majorAxis, float minorAxis)
    {
        a = majorAxis;
        b = minorAxis;
        maxAxis = Mathf.Max(a, b);
    }
    public void SetAxies(float majorAxis)
    {
        a = majorAxis;
        b = majorAxis * setting.ellipseRatio;
        maxAxis = Mathf.Max(a, b);
    }
    public void SetAxies(float majorAxis, Vector2 pos)
    {
        a = majorAxis;
        b = majorAxis * setting.ellipseRatio;
        maxAxis = Mathf.Max(a, b);
        position = pos;
    }

    public static Vector2 GetPoint(float a, float b, Vector2 zeroPos, float radian)
    {
        float t = Mathf.Atan2(a * Mathf.Sin(radian), b * Mathf.Cos(radian));
        return new Vector2(zeroPos.x + a * Mathf.Cos(t), zeroPos.y + b * Mathf.Sin(t));
    }

    public static Vector2 GetPoint(Ellipse ellipse, float radian)
    {
        return GetPoint(ellipse.a, ellipse.b, ellipse.position, radian);
    }

    public static bool IsCollided(Ellipse ellipse1, Ellipse ellipse2)
    {
        return CollisionDepth(ellipse1, ellipse2) > 0f;
    }

    public static float CollisionDepth(Ellipse ellipse1, Ellipse ellipse2)
    {
        if ((ellipse1.position - ellipse2.position).magnitude > ellipse1.maxAxis + ellipse2.maxAxis)
            return -1f;

        var direc = ellipse2.position - ellipse1.position;
        var angle = Mathf.Atan2(direc.y, direc.x);
        var radius1 = (GetPoint(ellipse1, angle) - ellipse1.position).magnitude;
        var radius2 = (GetPoint(ellipse2, angle + Mathf.PI) - ellipse2.position).magnitude;

        return (radius1 + radius2) - direc.magnitude;
    }

    public static bool IsPointInEllipse(Ellipse ellipse, Vector2 point)
    {
        var pointDiff = point - ellipse.position;
        return Mathf.Pow(pointDiff.x, 2f) / Mathf.Pow(ellipse.a, 2)
            + Mathf.Pow(pointDiff.y, 2f) / Mathf.Pow(ellipse.b, 2)
            <= 1f;
    }
}