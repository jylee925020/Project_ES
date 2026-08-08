using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float lifetime = 0.7f;
    [SerializeField] private float moveSpeed = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float fadeStartRatio = 0.5f;

    private float elapsedTime;
    private Color initialColor;

    private void Awake()
    {
        initialColor = text.color;
    }

    public void Initialize(int damage)
    {
        text.text = damage.ToString();

        elapsedTime = 0f;
        text.color = initialColor;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        float progress = elapsedTime / lifetime;

        if (progress >= fadeStartRatio)
        {
            float fadeProgress =
                Mathf.InverseLerp(fadeStartRatio, 1f, progress);

            Color color = initialColor;
            color.a = 1f - fadeProgress;
            text.color = color;
        }

        if (elapsedTime >= lifetime)
            Destroy(gameObject);
    }
}