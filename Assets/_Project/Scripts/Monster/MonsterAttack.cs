using UnityEngine;
/// <summary>
/// 몬스터의 공격을 담당한다.
/// - 공격 쿨타임 관리
/// - 공격 중 여부 관리
/// - MonsterAI가 공격을 요청했을 때 가능한 경우에만 공격 시작
/// </summary>
public class MonsterAttack : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDuration = 0.3f;

    [Header("Attack Object")]
    [SerializeField] private AttackObject attackPrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int attackDamage = 1;

    public bool IsAttacking { get; private set; }

    private float cooldownTimer;
    private float attackTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!IsAttacking)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            EndAttack();
        }
    }

    public bool TryAttack()
    {
        if (IsAttacking || cooldownTimer > 0f)
            return false;

        StartAttack();
        return true;
    }

    private void StartAttack()
    {
        IsAttacking = true;
        attackTimer = attackDuration;
        cooldownTimer = attackCooldown;

        SpawnAttackObject();
    }

    private void SpawnAttackObject()
    {
        AttackObject.SpawnAsChild(
            attackPrefab,
            attackPoint,
            attackDamage,
            AttackFaction.Monster
        );
    }

    private void EndAttack()
    {
        IsAttacking = false;
    }
}