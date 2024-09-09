using UnityEngine;
using UnityEngine.InputSystem;

public class EllipseTester : MonoBehaviour
{
    public EllipseDrawer a;
    public EllipseDrawer b;
    public SpriteRenderer aPoint;
    public SpriteRenderer bPoint;
    public LineRenderer line;

    private void Awake()
    {
        a.ellipse = new(3f);
        b.ellipse = new(2f);
    }

    private void Update()
    {
        a.ellipse.position = a.transform.position;
        b.transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.value) + Vector3.forward * 10f;
        b.ellipse.position = b.transform.position;


        aPoint.transform.position = a.ellipse.GetNearPoint(b.ellipse.position);
        bPoint.transform.position = b.ellipse.GetNearPoint(a.ellipse.position);

        line.SetPositions(new Vector3[] { a.ellipse.position, b.ellipse.position });

        if(a.ellipse.IsCollidedWith(b.ellipse))
            bPoint.color = aPoint.color = Color.red;
        else
            bPoint.color = aPoint.color = Color.white;
    }
}
