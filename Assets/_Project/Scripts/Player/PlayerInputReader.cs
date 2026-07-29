using UnityEngine;

/// <summary>
/// 플레이어 입력값을 조회할 수 있는 클래스
/// 좌우, 점프, qwer에 대해 Pressed, Held, Released를 알려줌.
/// 좌우를 둘 다 누를 땐 나중에 누른 방향으로 이동해야 한다고 알려줌.
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public bool LeftHeld => inputActions.Player.MoveLeft.IsPressed();
    public bool RightHeld => inputActions.Player.MoveRight.IsPressed();

    public bool LeftPressed =>
        inputActions.Player.MoveLeft.WasPressedThisFrame();

    public bool RightPressed =>
        inputActions.Player.MoveRight.WasPressedThisFrame();

    public bool JumpHeld =>
        inputActions.Player.Jump.IsPressed();

    public bool JumpPressed =>
        inputActions.Player.Jump.WasPressedThisFrame();

    public bool JumpReleased =>
        inputActions.Player.Jump.WasReleasedThisFrame();

    public bool BothDirectionsHeld => LeftHeld && RightHeld;    // 좌우 둘 다 눌렀는지
    private float lastPressedDirection;     // 좌우를 둘 다 눌렀을 때 둘 중 무엇을 나중에 눌렀는지

    // 어디로 이동해야 하는지(좌/우/둘다 눌렀을 땐 나중에 누른 방향)
    public float MoveDirection
    {
        get
        {
            if (BothDirectionsHeld)
                return lastPressedDirection;

            if (LeftHeld)
                return -1f;

            if (RightHeld)
                return 1f;

            return 0f;
        }
    }

    #region Weapon Input

    public bool WeaponQPressed =>
        inputActions.Player.WeaponQ.WasPressedThisFrame();

    public bool WeaponWPressed =>
        inputActions.Player.WeaponW.WasPressedThisFrame();

    public bool WeaponEPressed =>
        inputActions.Player.WeaponE.WasPressedThisFrame();

    public bool WeaponRPressed =>
        inputActions.Player.WeaponR.WasPressedThisFrame();

    #endregion

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void Update()
    {
        UpdateLastPressedDirection();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void UpdateLastPressedDirection()
    {
        if (LeftPressed)
            lastPressedDirection = -1f;

        if (RightPressed)
            lastPressedDirection = 1f;
    }
}