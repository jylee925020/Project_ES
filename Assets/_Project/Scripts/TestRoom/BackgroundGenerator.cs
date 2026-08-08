using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private SpriteRenderer rectanglePrefab;
    [SerializeField] private int count = 20;
    [SerializeField] private float horizontalRange = 20f;

    [Header("Rectangle Size")]
    [SerializeField] private Vector2 widthRange = new(1f, 4f);
    [SerializeField] private Vector2 heightRange = new(2f, 8f);

    [Header("Color")]
    [SerializeField, Range(0, 255)] private int minBrightness = 0;
    [SerializeField, Range(0, 255)] private int maxBrightness = 100;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        for (int i = 0; i < count; i++)
        {
            float width = Random.Range(widthRange.x, widthRange.y);
            float height = Random.Range(heightRange.x, heightRange.y);

            float x = transform.position.x
                      + Random.Range(-horizontalRange * 0.5f, horizontalRange * 0.5f);

            // 피벗이 중앙이라고 가정.
            // 사각형의 아래쪽이 생성기의 Y와 일치하도록 높이의 절반만큼 올린다.
            float y = transform.position.y + height * 0.5f;

            SpriteRenderer rectangle = Instantiate(
                rectanglePrefab,
                new Vector3(x, transform.position.y, transform.position.z),
                Quaternion.identity,
                transform
            );

            rectangle.transform.localScale = new Vector3(width, height, 1f);

            // 실제 생성된 사각형의 월드 높이를 기준으로 밑면을 지평선에 맞춘다.
            float actualHeight = rectangle.bounds.size.y;

            rectangle.transform.position = new Vector3(
                x,
                transform.position.y + actualHeight * 0.5f,
                transform.position.z
            );

            byte n = (byte)Random.Range(minBrightness, maxBrightness + 1);
            rectangle.color = new Color32(n, n, n, 255);
        }
    }
}