using System;
using UnityEngine;

/// <summary>
/// 플레이어의 물리 처리를 담당하는 클래스.
/// Rigidbody2D 속도 제어와 현재의 임시 지면 감지를 수행한다.
/// </summary>
public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerState state;

    public event Action OnLanded;

    public float CurrentVelocityX => rb.linearVelocity.x;
    public float CurrentVelocityY => rb.linearVelocity.y;

    private void Update()
    {
        CheckGrounded();
    }

    #region Ground Check

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    // 기존 코드와의 호환을 위해 아직 유지한다.
    public bool IsGrounded { get; private set; }

    private bool groundStateInitialized;

    private void CheckGrounded()
    {
        bool newIsGrounded = Physics2D.OverlapBox(
            groundCheckPoint.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        // 최초 검사에서는 착지 이벤트를 발생시키지 않는다.
        if (!groundStateInitialized)
        {
            IsGrounded = newIsGrounded;
            state.SetGrounded(newIsGrounded);

            groundStateInitialized = true;
            return;
        }

        bool landedThisFrame = !IsGrounded && newIsGrounded;

        // 상태를 먼저 갱신한 뒤 이벤트를 발생시킨다.
        // 이벤트 수신자가 현재 접지 상태를 읽을 때 최신 값을 얻을 수 있다.
        IsGrounded = newIsGrounded;
        state.SetGrounded(newIsGrounded);

        if (landedThisFrame)
        {
            OnLanded?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            groundCheckPoint.position,
            groundCheckSize
        );
    }

    #endregion


    #region Velocity Control

    public void SetVelocityX(float velocityX)
    {
        rb.linearVelocity = new Vector2(
            velocityX,
            rb.linearVelocity.y
        );
    }

    public void SetVelocityY(float velocityY)
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            velocityY
        );
    }

    public void MoveVelocityX(
        float targetVelocityX,
        float changeSpeed
    )
    {
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetVelocityX,
            changeSpeed * Time.deltaTime
        );

        SetVelocityX(newVelocityX);
    }

    #endregion
}