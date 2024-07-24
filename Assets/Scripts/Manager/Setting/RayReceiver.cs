using UnityEngine;
using UnityEngine.EventSystems;

public class RayReceiver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool Received { get; private set; }
    public bool ReceivedLastFrame { get; private set; }

    public void Update()
    {
        ReceivedLastFrame = Received;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Received = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Received = false;
    }
}
