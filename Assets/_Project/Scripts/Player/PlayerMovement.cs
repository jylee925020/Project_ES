using System;
using UnityEngine;

/// <summary>
/// 이동, 점프, 방향 전환 등 플레이어의 움직임을 제어하는 클래스
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerPhysics physics;

    #region Move
    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 80f;


    // canChangeFacing을 false로 설정하면 방향 전환 없이 이동만 함
    public void Move(float inputX, bool canChangeFacing = true)
    {
        inputX = Mathf.Clamp(inputX, -1f, 1f);

        if (canChangeFacing)
        {
            UpdateFacing(inputX);
        }

        if (Mathf.Abs(inputX) > 0.01f)
        {
            float targetVelocityX = inputX * moveSpeed;
            physics.MoveVelocityX(targetVelocityX, acceleration);
        }
        else
        {
            physics.MoveVelocityX(0f, deceleration);
        }
    }

    // 즉시 좌우 속도를 0으로 만들어 이동을 멈춤
    public void StopImmediately()
    {
        physics.SetVelocityX(0f);
    }

    #endregion

    #region Jump

    [Header("Jump")]
    [SerializeField] private float jumpPower = 12f;
    [SerializeField, Range(0f, 1f)] private float jumpCutRate = 0.5f;

    public event Action OnJumped;

    public void Jump()
    {
        if (!physics.IsGrounded)
            return;

        physics.SetVelocityY(jumpPower);
        OnJumped?.Invoke();
    }

    public void CutJump()
    {
        if (physics.CurrentVelocityY <= 0f)
            return;

        physics.SetVelocityY(physics.CurrentVelocityY * jumpCutRate);
    }

    #endregion

    #region Facing

    [Header("Facing")]
    [SerializeField] private Transform playerRoot;

    public bool IsFacingRight { get; private set; } = true;

    private void UpdateFacing(float inputX)
    {
        if (inputX > 0f)
            FaceRight();
        else if (inputX < 0f)
            FaceLeft();
    }

    private void FaceRight()
    {
        if (IsFacingRight)
            return;

        IsFacingRight = true;
        SetRootScaleX(1f);
    }

    private void FaceLeft()
    {
        if (!IsFacingRight)
            return;

        IsFacingRight = false;
        SetRootScaleX(-1f);
    }

    private void SetRootScaleX(float sign)
    {
        Vector3 scale = playerRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * sign;
        playerRoot.localScale = scale;
    }

    #endregion
}