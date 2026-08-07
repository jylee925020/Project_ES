using UnityEngine;

/// <summary>
/// 몬스터의 공격을 담당한다.
/// - 공격 선딜 관리
/// - 공격 지속 시간 관리
/// - 공격 쿨타임 관리
/// - 공격 판정 생성
/// </summary>
public class MonsterAttack : MonoBehaviour
{
    private enum AttackPhase
    {
        None,
        Startup,
        Active
    }

    [Header("Timing")]
    [SerializeField] private float attackStartupDuration = 0.2f;
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float attackCooldown = 1f;

    private BoxHitBoxSpawner hitBoxSpawner;
    private AttackHitBox currentHitBox;

    [SerializeField] private VFXSpawner attackVFXSpawner;

    [SerializeField] private int attackDamage = 1;

    public bool IsAttacking => currentPhase != AttackPhase.None;
    public bool IsInStartup => currentPhase == AttackPhase.Startup;
    public bool IsActive => currentPhase == AttackPhase.Active;

    private AttackPhase currentPhase;
    private float phaseTimer;
    private float cooldownTimer;


    private void Awake()
    {
        hitBoxSpawner = GetComponent<BoxHitBoxSpawner>();
    }

    private void Update()
    {
        UpdateCooldown();
        UpdateAttack();
    }

    private bool attackEnabled = true;

    public void SetAttackEnabled(bool enabled)
    {
        attackEnabled = enabled;

        if (!enabled)
            CancelAttack();
    }

    public bool TryAttack()
    {
        if (!attackEnabled)
            return false;

        if (IsAttacking || cooldownTimer > 0f)
            return false;

        StartAttack();
        return true;
    }

    private void CancelAttack()
    {
        DisableCurrentHitBox();

        currentPhase = AttackPhase.None;
        phaseTimer = 0f;
    }

    private void StartAttack()
    {
        currentPhase = AttackPhase.Startup;
        phaseTimer = attackStartupDuration;
        cooldownTimer = attackCooldown;
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void UpdateAttack()
    {
        if (!IsAttacking)
            return;

        phaseTimer -= Time.deltaTime;

        if (phaseTimer > 0f)
            return;

        switch (currentPhase)
        {
            case AttackPhase.Startup:
                StartActive();
                break;

            case AttackPhase.Active:
                EndAttack();
                break;
        }
    }

    private void StartActive()
    {
        currentPhase = AttackPhase.Active;
        phaseTimer = attackDuration;

        SpawnAttackObject();
        SpawnAttackVFX();
    }
    private void SpawnAttackVFX()
    {
        if (attackVFXSpawner == null)
            return;

        attackVFXSpawner.Spawn();
    }
    private void SpawnAttackObject()
    {
        if (hitBoxSpawner == null)
            return;

        currentHitBox = hitBoxSpawner.Spawn(
            attackDamage,
            AttackFaction.Monster
        );
    }

    private void EndAttack()
    {
        DisableCurrentHitBox();
        currentPhase = AttackPhase.None;
    }

    private void DisableCurrentHitBox()
    {
        if (currentHitBox == null)
            return;

        currentHitBox.DisableDamage();
        currentHitBox = null;
    }

}