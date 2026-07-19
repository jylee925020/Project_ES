using UnityEngine;

public class MonsterAI : MonoBehaviour
{

    [Header("Patrol")]
    [SerializeField] private bool patrolEnabled = true;

    [Header("Wall Detection")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.4f;

    public float MoveDirection { get; private set; } = 1f;

    private bool isTurnBlocked;

    public bool PatrolEnabled => patrolEnabled;


    private void Update()
    {
        if (!patrolEnabled)
        {
            isTurnBlocked = false;
            return;
        }

        UpdatePatrol();
    }

    // raycast를 이용하여 벽과 땅을 감지하고, 벽이 있거나 땅이 없으면 방향을 바꾼다.
    private void UpdatePatrol()
    {
        bool wallAhead = IsWallAhead();
        bool groundAhead = IsGroundAhead();

        bool shouldTurnAround = wallAhead || !groundAhead;

        if (shouldTurnAround && !isTurnBlocked)
        {
            TurnAround();
            isTurnBlocked = true;
        }
        else if (!shouldTurnAround)
        {
            isTurnBlocked = false;
        }
    }

    // 순찰 활성화/비활성화
    public void SetPatrolEnabled(bool enabled)
    {
        patrolEnabled = enabled;

        if (!enabled)
        {
            isTurnBlocked = false;
        }
    }

    private void TurnAround()
    {
        MoveDirection *= -1f;
    }

    // 전방에 벽이 있으면 true
    private bool IsWallAhead()
    {
        Vector2 direction = Vector2.right * MoveDirection;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position,
            direction,
            wallCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            wallCheck.position,
            direction * wallCheckDistance,
            hit.collider != null ? Color.red : Color.green
        );

        return hit.collider != null;
    }

    // 전방에 땅이 있으면 true
    private bool IsGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            groundCheck.position,
            Vector2.down * groundCheckDistance,
            hit.collider != null ? Color.green : Color.red
        );

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (wallCheck == null)
            return;

        Gizmos.color = Color.red;

        Vector3 direction =
            Vector3.right * MoveDirection * wallCheckDistance;

        Gizmos.DrawLine(
            wallCheck.position,
            wallCheck.position + direction
        );
    }
}