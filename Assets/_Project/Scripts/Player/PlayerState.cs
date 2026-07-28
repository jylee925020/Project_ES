using UnityEngine;
using System;

// 플레이어의 현재 행동 상태를 나타내는 열거형
public enum PlayerActionType
{
    None,
    Attack,
    HitStun,
    Roll,
    Dead
}

/// <summary>
/// 플레이어의 현재 상태를 저장하고,
/// 다른 플레이어 컴포넌트가 공통으로 조회할 수 있도록 제공하는 클래스.
/// 상태 변화 이벤트를 발생시켜 다른 컴포넌트가 상태 변화에 반응할 수 있도록 한다.
/// </summary>
public class PlayerState : MonoBehaviour
{
    #region Grounded

    public event Action OnLanded;
    public event Action OnLeftGround;

    public bool IsGrounded { get; private set; }

    public void InitializeGrounded(bool isGrounded)
    {
        IsGrounded = isGrounded;
    }

    public void SetGrounded(bool isGrounded)
    {
        if (IsGrounded == isGrounded)
            return;

        bool wasGrounded = IsGrounded;
        IsGrounded = isGrounded;

        if (!wasGrounded && isGrounded)
        {
            OnLanded?.Invoke();
        }
        else
        {
            OnLeftGround?.Invoke();
        }
    }

    #endregion


    #region Facing

    public event Action<bool> OnFacingChanged; // isFacingRight 값도 같이 전달함.

    public bool IsFacingRight { get; private set; } = true;

    public void SetFacingRight(bool isFacingRight)
    {
        if (IsFacingRight == isFacingRight)
            return;

        IsFacingRight = isFacingRight;
        OnFacingChanged?.Invoke(isFacingRight);
    }

    #endregion


    #region Action

    public event Action<PlayerActionType> OnActionChanged;

    public PlayerActionType CurrentAction { get; private set; }
        = PlayerActionType.None;

    public bool IsAttacking =>
        CurrentAction == PlayerActionType.Attack;

    public bool IsInHitStun =>
        CurrentAction == PlayerActionType.HitStun;

    public bool IsDead =>
        CurrentAction == PlayerActionType.Dead;

    // 새로운 액션을 시작함. 액션이 바뀐다면 이벤트 발행.
    public void BeginAction(PlayerActionType action)
    {
        if (CurrentAction == action)
            return;
        CurrentAction = action;
        OnActionChanged?.Invoke(CurrentAction);
    }

    // 액션 종료도 액션 변경 이벤트를 발생시킴.
    public void EndAction(PlayerActionType action)
    {
        if (CurrentAction != action)
            return;

        CurrentAction = PlayerActionType.None;
        OnActionChanged?.Invoke(CurrentAction);
    }

    // 사망도 액션 변경 이벤트 발생
    public void SetDead()
    {
        if (CurrentAction == PlayerActionType.Dead)
            return;

        CurrentAction = PlayerActionType.Dead;
        OnActionChanged?.Invoke(CurrentAction);
    }

    #endregion

    // 행동 제한을 계산해주는 프로퍼티
    #region Restrictions

    public bool CanMove =>
    !IsDead &&
    !IsInHitStun &&
    !(IsAttacking && IsGrounded);

    public bool CanChangeFacing =>
        !IsDead &&
        !IsInHitStun &&
        !IsAttacking;

    public bool CanJump =>
        !IsDead &&
        !IsInHitStun &&
        !IsAttacking;

    public bool CanAttack =>
        !IsDead &&
        !IsInHitStun &&
        CurrentAction == PlayerActionType.None;

    // 사망시, 지상 공격시에만 속도 0으로
    public bool ShouldStopHorizontalMovement =>
        IsDead ||
        (IsAttacking && IsGrounded);

    #endregion
}

