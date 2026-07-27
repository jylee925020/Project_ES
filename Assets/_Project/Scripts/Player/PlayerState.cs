using UnityEngine;

/// <summary>
/// 플레이어의 현재 상태를 저장하고,
/// 다른 플레이어 컴포넌트가 공통으로 조회할 수 있도록 제공하는 클래스.
/// </summary>\
public enum PlayerActionType
{
    None,
    Attack,
    HitStun,
    Roll,
    Dead
}
public class PlayerState : MonoBehaviour
{
    #region Grounded

    public bool IsGrounded { get; private set; }

    public void SetGrounded(bool isGrounded)
    {
        IsGrounded = isGrounded;
    }

    #endregion


    #region Facing

    public bool IsFacingRight { get; private set; } = true;

    public void SetFacingRight(bool isFacingRight)
    {
        IsFacingRight = isFacingRight;
    }

    #endregion


    #region Action

    public PlayerActionType CurrentAction { get; private set; }
        = PlayerActionType.None;

    public bool IsAttacking =>
        CurrentAction == PlayerActionType.Attack;

    public bool IsInHitStun =>
        CurrentAction == PlayerActionType.HitStun;

    public bool IsDead =>
        CurrentAction == PlayerActionType.Dead;

    public void BeginAction(PlayerActionType action)
    {
        CurrentAction = action;
    }

    public void EndAction(PlayerActionType action)
    {
        if (CurrentAction != action)
            return;

        CurrentAction = PlayerActionType.None;
    }

    public void SetDead()
    {
        CurrentAction = PlayerActionType.Dead;
    }

    #endregion


    #region Restrictions

    public bool CanMove
    {
        get
        {
            if (IsDead || IsInHitStun)
                return false;

            if (CurrentAction == PlayerActionType.Attack && IsGrounded)
                return false;

            return true;
        }
    }

    public bool CanChangeFacing
    {
        get
        {
            if (IsDead || IsInHitStun)
                return false;

            if (CurrentAction == PlayerActionType.Attack)
                return false;

            return true;
        }
    }

    public bool CanJump
    {
        get
        {
            if (IsDead || IsInHitStun)
                return false;

            if (CurrentAction == PlayerActionType.Attack)
                return false;

            return true;
        }
    }

    public bool CanAttack
    {
        get
        {
            if (IsDead || IsInHitStun)
                return false;

            return CurrentAction != PlayerActionType.Attack;
        }
    }

    #endregion
}

