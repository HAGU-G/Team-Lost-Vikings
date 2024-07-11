using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipseDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Start()
    {
        Draw(8, 6);
    }

    public void Draw(float a, float b)
    {
        lineRenderer.positionCount = 32;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, GetPoint(a, b, Vector2.zero, 2 * Mathf.PI / lineRenderer.positionCount * i));
        }

        lineRenderer2.positionCount = 2;
    }

    public static Vector2 GetPoint(float a, float b, Vector2 zeroPos, float radian)
    {
        return new Vector2(zeroPos.x + a * Mathf.Cos(radian), zeroPos.y + b * Mathf.Sin(radian));
    }

    public static Vector2 GetNearPoint(float a, float b, Vector2 zeroPos, Vector2 point)
    {
        float x0 = point.x - zeroPos.x;
        float y0 = point.y - zeroPos.y;

        // 초기 추정치
        float x = a, y = 0;
        float tol = 1e-6f;
        int maxIter = 1000;

        for (int iter = 0; iter < maxIter; iter++)
        {
            float alpha = (x0 * x0) / (a * a) + (y0 * y0) / (b * b);

            // 새로운 점 계산
            float newX = a * x0 / Mathf.Sqrt(alpha);
            float newY = b * y0 / Mathf.Sqrt(alpha);

            // 수렴 확인
            if (Mathf.Abs(newX - x) < tol && Mathf.Abs(newY - y) < tol)
            {
                x = newX;
                y = newY;
                break;
            }

            x = newX;
            y = newY;
        }

        return new Vector2(x + zeroPos.x, y + zeroPos.y);
    }

    private void Update()
    {
        lineRenderer2.SetPosition(0, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer2.SetPosition(1, Vector2.zero);

    }
}
