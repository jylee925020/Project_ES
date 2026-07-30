using UnityEngine;

/// <summary>
/// 플레이어의 입력과 현재 상태를 바탕으로
/// 이동, 점프, 공격 등의 행동을 결정한다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerInputReader inputReader;
    private PlayerMovement movement;
    private PlayerCombat combat;
    private PlayerState state;

    #region Lifecycle

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        state = GetComponent<PlayerState>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCombat();

        // 공격 강제 종료 테스트
        if (Input.GetKeyDown(KeyCode.T))
        {
            combat.ForceInterruptCurrentAction();
        }
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        if (!state.CanMove)
        {
            if (state.ShouldStopHorizontalMovement)
                movement.StopImmediately();

            return;
        }

        bool canChangeFacing =
            state.CanChangeFacing &&
            !inputReader.BothDirectionsHeld;

        movement.Move(
            inputReader.MoveDirection,
            canChangeFacing);
    }

    #endregion

    #region Jump

    private void HandleJump()
    {
        if (inputReader.JumpReleased)
        {
            movement.CutJump();
        }

        if (inputReader.JumpPressed && state.CanJump)
        {
            movement.Jump();
        }
    }

    #endregion

    #region Combat

    private void HandleCombat()
    {
        if (inputReader.WeaponQPressed)
            combat.TryUseSlot(0);

        if (inputReader.WeaponWPressed)
            combat.TryUseSlot(1);

        if (inputReader.WeaponEPressed)
            combat.TryUseSlot(2);

        if (inputReader.WeaponRPressed)
            combat.TryUseSlot(3);
    }

    #endregion
}