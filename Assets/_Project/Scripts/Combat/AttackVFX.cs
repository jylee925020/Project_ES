using UnityEngine;

/// <summary>
/// 일회성 공격 이펙트의 수명과 투명도를 관리한다.
/// </summary>
public class AttackVFX : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 0.3f;

    [Header("Alpha")]
    [Tooltip("가로축은 정규화된 수명(0~1), 세로축은 원본 알파에 곱할 값")]
    [SerializeField]
    private AnimationCurve alphaCurve =
        AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float elapsedTime;
    private void Awake()
    {
        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>(true);

        if (spriteRenderer == null)
        {
            Debug.LogError(
                $"{name}: SpriteRenderer가 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;

        float normalizedTime =
            Mathf.Clamp01(elapsedTime / lifetime);

        float alphaMultiplier =
            Mathf.Max(0f, alphaCurve.Evaluate(normalizedTime));

        Color color = originalColor;
        color.a = originalColor.a * alphaMultiplier;

        spriteRenderer.color = color;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    public void SetColor(Color color)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponentInChildren<SpriteRenderer>(true);
        }

        originalColor = color;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    public void Stop()
    {
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        lifetime = Mathf.Max(0f, lifetime);
    }
}