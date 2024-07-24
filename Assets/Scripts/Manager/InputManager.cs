using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputManager : MonoBehaviour
{
    public RayReceiver receiver;

    public bool Tap { get; private set; }
    public bool Touch { get; private set; }
    public bool Moved { get; private set; }
    public Vector2 Pos { get; private set; }
    public Vector3 WorldPos => Camera.main.ScreenToWorldPoint(Pos);
    public Vector2 DeltaPos { get; private set; }
    public Vector3 WorldDeltaPos { get; private set; }
    public Vector2 PrevPos { get; private set; }
    public float MoveDistance { get; private set; }
    public bool Press {  get; private set; }

    private float tapAllowInch = 0.2f;
    private Finger firstID;

    private void Awake()
    {
        if (GameManager.inputManager != null)
        {
            Destroy(gameObject);
            return;
        }
        GameManager.inputManager = this;

        EnhancedTouchSupport.Enable();
    }

    private void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        Tap = false;
        Touch = false;
        Press = false;

        if (Touchscreen.current != null)
        {
            int touchCount = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
            if (touchCount > 0)
            {
                Touch = true;

                foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
                {
                    if (firstID == touch.finger)
                        Pos = touch.screenPosition;
                    bool outTapDistance = false;
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (firstID == null)
                            {
                                firstID = touch.finger;
                                Pos = touch.screenPosition;
                                PrevPos = touch.screenPosition;
                                Press = true;
                            }
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            if (firstID != touch.finger)
                                break;
                            DeltaPos = touch.delta;
                            Moved = DeltaPos != Vector2.zero;
                            MoveDistance += DeltaPos.magnitude;
                            outTapDistance = MoveDistance * Screen.dpi > tapAllowInch;
                            WorldDeltaPos = Camera.main.ScreenToWorldPoint(Pos) - Camera.main.ScreenToWorldPoint(PrevPos);
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (firstID != touch.finger)
                                break;
                            Tap = MoveDistance <= tapAllowInch * Screen.dpi && !outTapDistance;
                            firstID = null;
                            Moved = false;
                            MoveDistance = 0f;
                            break;
                    }
                    if (firstID == touch.finger)
                        PrevPos = touch.screenPosition;
                }
            }
        }
        else
        {
            MouseInput();
        }
    }

    private void MouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null)
            return;

        bool outTapDistance = false;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Pos = PrevPos = mouse.position.value;
            Touch = true;
        }
        if (mouse.leftButton.isPressed)
        {
            Touch = true;
            Pos = mouse.position.value;

            DeltaPos = Pos - PrevPos;
            Moved = DeltaPos != Vector2.zero;
            MoveDistance += DeltaPos.magnitude;
            outTapDistance = MoveDistance * Screen.dpi > tapAllowInch;
            WorldDeltaPos = Camera.main.ScreenToWorldPoint(Pos) - Camera.main.ScreenToWorldPoint(PrevPos);
            PrevPos = mouse.position.value;
        }
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            Tap = MoveDistance <= tapAllowInch * Screen.dpi && !outTapDistance;
            Moved = false;
            MoveDistance = 0f;
        }
    }
}
