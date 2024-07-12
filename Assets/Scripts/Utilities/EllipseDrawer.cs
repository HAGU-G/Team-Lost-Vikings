using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipseDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Ellipse ellipse;
    public EllipseDrawer other;
    public GameObject point;

    private void Awake()
    {
        GameStarter.Instance.SetActiveOnComplete(gameObject);
    }

    private void Start()
    {
        Draw(ellipse.a, ellipse.b, Vector2.zero, lineRenderer);
    }

    public void Draw(float a, float b, Vector2 pos, LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 32;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, Ellipse.GetPoint(a, b, pos, 2 * Mathf.PI / lineRenderer.positionCount * i));
        }
    }

    public void Update()
    {
        ellipse.position = transform.position;
        Draw(ellipse.a, ellipse.b, ellipse.position, lineRenderer);
        if (ellipse.IsCollisoinedWith(other.ellipse,point) <= 0f)
        {
            Debug.Log($"{name}, {other.name} 충돌!");
        }
    }

}
