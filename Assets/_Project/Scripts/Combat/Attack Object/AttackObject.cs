/// <summary>
/// 공격 오브젝트의 수명, 시각 효과, 피격 판정을 담당한다.
/// - 공격 오브젝트의 수명 관리
/// - 공격 오브젝트의 시각 효과 관리
/// - 공격 오브젝트의 피격 판정 관리
/// </summary>
using System.Collections.Generic;
using UnityEngine;

// 공격 주체의 구분. 시전하는 측에서 결정함.
public enum AttackFaction
{
    Player,
    Monster
}

public class AttackObject : MonoBehaviour
{

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 1f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float elapsedTime;

    private bool isInitialized;

    private bool canDamage;

    private readonly List<Collider2D> overlapResults = new();
    private Collider2D attackCollider;
    private ContactFilter2D contactFilter;

    private readonly HashSet<IHitReceiver> hitReceivers = new();    // 중복 피격 방지를 위한 피격 대상들의 HashSet
    private HitInfo hitInfo;

    #region lifecycle & initialization
    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        contactFilter = new ContactFilter2D
        {
            useTriggers = true
        };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (spriteRenderer != null && lifetime > 0f)
        {
            float progress = Mathf.Clamp01(elapsedTime / lifetime);

            Color color = originalColor;
            color.a = Mathf.Lerp(originalColor.a, 0f, progress);

            spriteRenderer.color = color;
        }

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    public void Initialize(int attackDamage, AttackFaction attackFaction)
    {
        hitInfo = new HitInfo(attackDamage, attackFaction);
        isInitialized = true;

        CheckCurrentOverlaps();
    }

    public void DisableDamage()
    {
        canDamage = false;

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void CheckCurrentOverlaps()
    {
        Physics2D.SyncTransforms();

        overlapResults.Clear();
        attackCollider.Overlap(contactFilter, overlapResults);

        foreach (Collider2D other in overlapResults)
        {
            TryDamage(other);
        }
    }

    #endregion

    private void TryDamage(Collider2D other)
    {
        if (!isInitialized)
            return;

        IHitReceiver hitReceiver = other.GetComponentInParent<IHitReceiver>();

        if (hitReceiver == null)
            return;

        if (!hitReceivers.Add(hitReceiver))
            return;

        hitReceiver.ReceiveHit(hitInfo);
    }

    #region Static Spawn Methods
    // 공격 오브젝트를 자식으로 생성함. (근접 등 부착되어 있어야 하는 공격 오브젝트)
    public static AttackObject SpawnAsChild(
        AttackObject prefab,
        Transform attackPoint,
        int damage,
        AttackFaction faction)
    {
        if (!ValidateSpawnArguments(prefab, attackPoint))
            return null;

        AttackObject instance = Instantiate(
            prefab,
            attackPoint
        );

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        instance.Initialize(damage, faction);

        return instance;
    }

    // 공격 오브젝트를 독립적으로 생성함. (원거리 등 독립적으로 존재해야 하는 공격 오브젝트)
    public static AttackObject SpawnIndependent(
        AttackObject prefab,
        Transform attackPoint,
        int damage,
        AttackFaction faction)
    {
        if (!ValidateSpawnArguments(prefab, attackPoint))
            return null;

        AttackObject instance = Instantiate(
            prefab,
            attackPoint.position,
            attackPoint.rotation
        );

        instance.Initialize(damage, faction);

        return instance;
    }

    // 생성 인자 유효성 검사
    private static bool ValidateSpawnArguments(
        AttackObject prefab,
        Transform attackPoint)
    {
        if (prefab == null)
        {
            Debug.LogError("생성할 AttackObject 프리팹이 없습니다.");
            return false;
        }

        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint가 지정되지 않았습니다.");
            return false;
        }

        return true;
    }
    #endregion
}
