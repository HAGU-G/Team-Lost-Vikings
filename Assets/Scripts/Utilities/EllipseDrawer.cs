using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EllipseDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Ellipse ellipse;
    public IStatUsable owner;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        owner = GetComponent<IStatUsable>();

        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        ellipse = owner.GetStats.SizeEllipse;
        Draw(ellipse.a, ellipse.b, Vector2.zero, lineRenderer);
        lineRenderer.useWorldSpace = false;
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
    }

}
