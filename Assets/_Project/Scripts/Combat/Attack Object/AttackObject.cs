using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 프리팹의 수명, 시각 효과, 피격 판정을 담당한다.
/// </summary>
public class AttackObject : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Vector2 spawnOffset;
    public Vector2 SpawnOffset => spawnOffset;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 1f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private readonly HashSet<MonsterHealth> damagedMonsters = new();    // 여러번 피격 방지

    private Color originalColor;
    private float elapsedTime;
    private int damage;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Initialize(int attackDamage)
    {
        damage = attackDamage;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (spriteRenderer != null)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        MonsterHealth monsterHealth =
            other.GetComponentInParent<MonsterHealth>();

        if (monsterHealth == null)
            return;

        if (!damagedMonsters.Add(monsterHealth))
            return;

        monsterHealth.TakeDamage(damage);
    }
}