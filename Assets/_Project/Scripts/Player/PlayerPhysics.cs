using UnityEngine;
/// <summary>
/// 플레이어의 물리 처리를 담당하는 클래스
/// 역학, 지면체크, 속도 제어 등을 수행
/// </summary>
public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

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
    private void CheckGrounded()
    {
        IsGrounded = Physics2D.OverlapBox(
            groundCheckPoint.position,
            groundCheckSize,
            0f,
            groundLayer
        );
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