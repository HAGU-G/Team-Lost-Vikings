using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputManager : MonoBehaviour
{
    public RayReceiver receiver;

    public bool Pressed { get; private set; }
    public bool Press { get; private set; }
    public bool Up { get; private set; }
    public bool Tap { get; private set; }
    public bool Moved { get; private set; }
    public float Zoom { get; private set; }

    public Vector2 Pos { get; private set; }
    public Vector3 WorldPos => Camera.main.ScreenToWorldPoint(Pos);
    public Vector2 DeltaPos { get; private set; }
    public Vector3 WorldDeltaPos { get; private set; }
    public Vector2 PrevPos { get; private set; }

    public Vector2 CenterPos { get; private set; }
    public Vector2 WorldCenterPos => Camera.main.ScreenToWorldPoint(CenterPos);
    public Vector2 CenterDeltaPos { get; private set; }
    public Vector3 WorldCenterDeltaPos { get; private set; }

    public float MoveDistance { get; private set; }

    private float tapAllowInch = 0.2f;
    private Finger firstID;
    private Finger secondID;

    public Vector2 SecondPos { get; private set; }
    public float TouchDistance { get; private set; }


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
        Pressed = false;
        Press = false;
        Up = false;
        Zoom = 0f;

        int touchCount = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
        if (touchCount > 0)
        {
            Pressed = true;

            CenterPos = Vector2.zero;
            CenterDeltaPos = Vector2.zero;
            WorldCenterDeltaPos = Vector2.zero;

            foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                if (firstID == touch.finger)
                    Pos = touch.screenPosition;

                bool isOutTapDistance = false;
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (firstID == null)
                        {
                            firstID = touch.finger;
                            Pos = touch.screenPosition;
                            PrevPos = touch.screenPosition;
                            Press = true;

                            CenterPos += Pos;
                        }
                        else if (secondID == null)
                        {
                            secondID = touch.finger;
                            SecondPos = touch.screenPosition;
                            TouchDistance = Vector2.Distance(Pos, SecondPos);

                            CenterPos += SecondPos;
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (firstID == touch.finger)
                        {
                            DeltaPos = touch.delta;
                            Moved = DeltaPos != Vector2.zero;
                            MoveDistance += DeltaPos.magnitude;
                            isOutTapDistance = MoveDistance * Screen.dpi > tapAllowInch;
                            WorldDeltaPos = Camera.main.ScreenToWorldPoint(Pos) - Camera.main.ScreenToWorldPoint(PrevPos);
                            
                            CenterDeltaPos += touch.delta;
                            WorldCenterDeltaPos += WorldDeltaPos;
                        }
                        if (secondID == touch.finger)
                        {
                            WorldCenterDeltaPos += (Camera.main.ScreenToWorldPoint(touch.screenPosition) - Camera.main.ScreenToWorldPoint(SecondPos));

                            SecondPos = touch.screenPosition;
                            if (TouchDistance > 0f)
                                Zoom = Vector2.Distance(Pos, SecondPos) - TouchDistance;
                            TouchDistance = Vector2.Distance(Pos, SecondPos);
                            
                            CenterDeltaPos += touch.delta;
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (firstID == touch.finger)
                        {
                            Tap = MoveDistance <= tapAllowInch * Screen.dpi && !isOutTapDistance;
                            firstID = null;
                            Moved = false;
                            Up = true;
                            MoveDistance = 0f;
                        }
                        if (secondID == touch.finger)
                        {
                            TouchDistance = 0f;
                            secondID = null;
                        }
                        break;
                }
                if (firstID == touch.finger)
                    PrevPos = touch.screenPosition;
            }

            CenterPos /= touchCount;
            CenterDeltaPos /= touchCount;
            WorldCenterDeltaPos /= touchCount;
        }
        MouseInput();
    }

    private void MouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null)
            return;

        bool outTapDistance = false;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            CenterPos = Pos = PrevPos = mouse.position.value;
            Pressed = true;
            Press = true;
        }
        if (mouse.leftButton.isPressed)
        {
            Pressed = true;
            CenterPos = Pos = mouse.position.value;

            CenterDeltaPos = DeltaPos = Pos - PrevPos;
            Moved = DeltaPos != Vector2.zero;
            MoveDistance += DeltaPos.magnitude;
            outTapDistance = MoveDistance * Screen.dpi > tapAllowInch;
            WorldCenterDeltaPos = WorldDeltaPos = Camera.main.ScreenToWorldPoint(Pos) - Camera.main.ScreenToWorldPoint(PrevPos);
            PrevPos = mouse.position.value;
        }
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            Tap = MoveDistance <= tapAllowInch * Screen.dpi && !outTapDistance;
            Moved = false;
            Up = true;
            MoveDistance = 0f;
        }

        if (Zoom == 0f)
            Zoom = mouse.scroll.value.y;
    }
}
