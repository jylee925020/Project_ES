using System.Collections;
using UnityEngine;

public class BulletTrailVFX : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float lifetime;

    public void Initialize(
        Vector2 start,
        Vector2 end,
        Color color,
        float thickness,
        float lifetime)
    {
        Vector2 delta = end - start;
        float length = delta.magnitude;

        transform.position = (start + end) * 0.5f;
        transform.right = delta.normalized;

        spriteRenderer.size =
            new Vector2(length, thickness);

        spriteRenderer.color = color;

        this.lifetime = lifetime;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        Color startColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;

            float t = lifetime <= 0f
                ? 1f
                : elapsed / lifetime;

            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, t);

            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}