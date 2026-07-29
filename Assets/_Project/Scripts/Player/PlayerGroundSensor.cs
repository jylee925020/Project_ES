using UnityEngine;

/// <summary>
/// 플레이어의 발밑을 검사하여 접지 여부를 감지하고,
/// 감지 결과를 PlayerState에 전달한다.
/// </summary>
public class PlayerGroundSensor : MonoBehaviour
{
    private PlayerState state;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField]
    private Vector2 groundCheckSize =
        new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    private bool groundStateInitialized;

    private void Awake()
    {
        state = GetComponent<PlayerState>();
    }

    private void Update()
    {
        UpdateGroundedState();
    }

    private void UpdateGroundedState()
    {
        bool isGrounded = CheckGrounded();

        // 최초 검사에서는 현재 상태만 맞추고,
        // 착지·탈지 이벤트는 발생시키지 않는다.
        if (!groundStateInitialized)
        {
            state.InitializeGrounded(isGrounded);
            groundStateInitialized = true;
            return;
        }

        state.SetGrounded(isGrounded);
    }

    private bool CheckGrounded()
    {
        return Physics2D.OverlapBox(
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
        Gizmos.DrawWireCube(
            groundCheckPoint.position,
            groundCheckSize
        );
    }
}