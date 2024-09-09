using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EllipseDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Unit owner = null;
    public Ellipse ellipse = null;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = false;

        owner = GetComponent<Unit>();
        if (owner != null)
            lineRenderer.material = owner.lineMaterial;
    }
    public void Draw(float a, float b, Vector2 pos, LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 100;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, Ellipse.GetPoint(a, b, pos, 2 * Mathf.PI / lineRenderer.positionCount * i));
        }
    }

    public void Update()
    {
        lineRenderer.enabled = GameSetting.Instance.isShowSizeEllipse;

        if (owner != null && owner.stats != null)
            Draw(owner.stats.SizeEllipse.a * 0.5f, owner.stats.SizeEllipse.b * 0.5f, Vector2.zero, lineRenderer);
        else if (ellipse != null)
            Draw(ellipse.a, ellipse.b, Vector2.zero, lineRenderer);
    }

}
