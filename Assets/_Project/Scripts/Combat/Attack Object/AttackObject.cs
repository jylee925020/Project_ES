using UnityEngine;

/// <summary>
/// 공격 프리팹의 기본 동작을 담당한다.
/// </summary>
public class AttackObject : MonoBehaviour
{

    [Header("Spawn")]
    [SerializeField] private Vector2 spawnOffset;
    public Vector2 SpawnOffset => spawnOffset;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 1f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;        // 투명화 전 색상 
    private float elapsedTime;          // 투명화 경과 시간

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float progress = elapsedTime / lifetime;

        Color color = originalColor;
        color.a = Mathf.Lerp(originalColor.a, 0f, progress);

        spriteRenderer.color = color;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}