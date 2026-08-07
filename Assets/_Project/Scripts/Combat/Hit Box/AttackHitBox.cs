using System.Collections.Generic;
using UnityEngine;

// 공격 주체의 구분. 시전하는 측에서 결정함.
public enum AttackFaction
{
    Player,
    Monster
}

/// <summary>
/// 공격 판정의 수명과 피격 전달을 담당한다.
/// - 공격 판정의 수명 관리
/// - 생성 시 이미 겹쳐 있는 대상 확인
/// - 중복 피격 방지
/// - HitInfo 전달
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class AttackHitBox : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 1f;

    private float elapsedTime;
    private bool isInitialized;

    private readonly List<Collider2D> overlapResults = new();
    private readonly HashSet<IHitReceiver> hitReceivers = new();

    private BoxCollider2D attackCollider;
    private ContactFilter2D contactFilter;
    private HitInfo hitInfo;

    private void Awake()
    {
        attackCollider = GetComponent<BoxCollider2D>();

        contactFilter = new ContactFilter2D
        {
            useTriggers = true
        };
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    public void Initialize(
        int attackDamage,
        AttackFaction attackFaction,
        Vector2 hitboxSize)
    {
        hitInfo = new HitInfo(
            attackDamage,
            attackFaction
        );

        attackCollider.size = hitboxSize;
        attackCollider.offset = Vector2.zero;

        isInitialized = true;

        CheckCurrentOverlaps();
    }

    public void DisableDamage()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void CheckCurrentOverlaps()
    {
        Physics2D.SyncTransforms();

        overlapResults.Clear();
        attackCollider.Overlap(
            contactFilter,
            overlapResults
        );

        foreach (Collider2D other in overlapResults)
        {
            TryDamage(other);
        }
    }

    private void TryDamage(Collider2D other)
    {
        if (!isInitialized)
            return;

        IHitReceiver hitReceiver =
            other.GetComponentInParent<IHitReceiver>();

        if (hitReceiver == null)
            return;

        if (!hitReceivers.Add(hitReceiver))
            return;

        hitReceiver.ReceiveHit(hitInfo);
    }

    public static AttackHitBox Spawn(
    AttackHitBox prefab,
    Vector3 worldPosition,
    Quaternion worldRotation,
    Vector2 size,
    int damage,
    AttackFaction faction)
    {
        if (prefab == null)
            return null;

        AttackHitBox instance = Instantiate(
            prefab,
            worldPosition,
            worldRotation
        );

        instance.transform.localScale = Vector3.one;
        instance.Initialize(damage, faction, size);

        return instance;
    }
   
}