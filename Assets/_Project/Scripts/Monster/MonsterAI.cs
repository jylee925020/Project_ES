using UnityEngine;
/// <summary>
/// 몬스터의 AI를 담당한다.
/// 상태는 4가지로 나뉜다.
/// Idle : 순찰도 안 하고 플레이어도 감지되지 않은 상태
/// Patrol : 순찰 중인 상태
/// Chase : 플레이어를 감지하고 추적 중인 상태
/// Attack : 플레이어를 감지하고 공격 중인 상태(이동 정지)
/// 추적 중 벽이나 낭떠러지를 만나면 그 자리에서 정지한다.
/// </summary>
public class MonsterAI : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Player Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("Patrol")]
    [SerializeField] private bool patrolEnabled = true;

    [Header("Wall Detection")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.4f;

    public MonsterState CurrentState { get; private set; }

    public float MoveDirection { get; private set; } = 1f;

    public bool ShouldMove { get; private set; }

    public bool PatrolEnabled => patrolEnabled;

    private bool isTurnBlocked;

    private MonsterAttack monsterAttack;

    private void Awake()
    {
        monsterAttack = GetComponent<MonsterAttack>();
    }


    // 상태를 업데이트하고, 상태에 따른 행동을 수행한다.
    private void Update()
    {
        UpdateState();
        UpdateBehaviour();
    }

    // 상태를 업데이트한다.(Idle, Patrol, Chase, Attack)
    private void UpdateState()
    {
        if (target == null)
        {
            CurrentState = patrolEnabled
                ? MonsterState.Patrol
                : MonsterState.Idle;

            return;
        }

        float distanceToTarget = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distanceToTarget <= attackRange)
        {
            CurrentState = MonsterState.Attack;
        }
        else if (distanceToTarget <= detectionRange)
        {
            CurrentState = MonsterState.Chase;
        }
        else
        {
            CurrentState = patrolEnabled
                ? MonsterState.Patrol
                : MonsterState.Idle;
        }
    }


    // 상태에 따라 행동한다. (Idle, Patrol, Chase, Attack)
    private void UpdateBehaviour()
    {
        switch (CurrentState)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;

            case MonsterState.Patrol:
                UpdatePatrol();
                break;

            case MonsterState.Chase:
                UpdateChase();
                break;

            case MonsterState.Attack:
                UpdateAttack();
                break;
        }
    }

    private void UpdateIdle()
    {
        ShouldMove = false;
        isTurnBlocked = false;
    }

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

        ShouldMove = true;
    }

    private void UpdateChase()
    {
        UpdateDirectionToTarget();

        bool wallAhead = IsWallAhead();
        bool groundAhead = IsGroundAhead();

        // 추적 중에는 장애물을 만났다고 반대 방향으로 도망가지 않고
        // 그 자리에서 정지한다.
        ShouldMove = !wallAhead && groundAhead;

        isTurnBlocked = false;
    }

    private void UpdateAttack()
    {
        UpdateDirectionToTarget();

        ShouldMove = false;
        isTurnBlocked = false;

        if (monsterAttack != null)
        {
            monsterAttack.TryAttack();
        }
    }

    private void UpdateDirectionToTarget()
    {
        if (target == null)
            return;

        float horizontalDifference =
            target.position.x - transform.position.x;

        if (Mathf.Abs(horizontalDifference) < 0.01f)
            return;

        MoveDirection = Mathf.Sign(horizontalDifference);
    }

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

    private bool IsWallAhead()
    {
        if (wallCheck == null)
            return false;

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

    private bool IsGroundAhead()
    {
        if (groundCheck == null)
            return true;

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

    private void OnValidate()
    {
        detectionRange = Mathf.Max(0f, detectionRange);
        attackRange = Mathf.Clamp(
            attackRange,
            0f,
            detectionRange
        );
    }

    // 감지 범위는 노란 원, 공격 범위는 빨간 원으로 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}