using System;
using UnityEngine;
/// <summary>
/// 플레이어의 물리 처리를 담당하는 클래스
/// 역학, 지면체크, 속도 제어 등을 수행
/// </summary>
public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    public event Action OnLanded;       // 착지 시 호출되는 이벤트

    public float CurrentVelocityX => rb.linearVelocity.x;
    public float CurrentVelocityY => rb.linearVelocity.y;
    private void Update()
    {
        CheckGrounded();
    }

    #region Ground Check
    // 지정한 오브젝트 주변으로 박스 형태의 충돌체를 생성하여 지면과의 충돌 여부를 확인
    // 땅 오브젝트의 레이어를 groundLayer로 설정해야 함
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }
    private bool groundStateInitialized;

    // 접지 상태 및 착지 이벤트 호출
    private void CheckGrounded()
    {
        // 발밑에 박스를 생성하여 지면과 충돌하는지 확인
        bool newIsGrounded = Physics2D.OverlapBox(
            groundCheckPoint.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        // 처음 체크 시에는 착지 이벤트를 호출하지 않음
        if (!groundStateInitialized)
        {
            IsGrounded = newIsGrounded;
            groundStateInitialized = true;
            return;
        }

        // 이전 프레임에는 공중, 이번 프레임에는 지면에 닿았다면 착지 이벤트 호출
        bool landedThisFrame = !IsGrounded && newIsGrounded;
        if (landedThisFrame)
        {
            OnLanded?.Invoke();
        }

        IsGrounded = newIsGrounded; // 접지 상태 업데이트

    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }

    #endregion


    #region Velocity Control
    public void SetVelocityX(float velocityX)
    {
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }

    public void SetVelocityY(float velocityY)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocityY);
    }

    public void MoveVelocityX(float targetVelocityX, float changeSpeed)
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