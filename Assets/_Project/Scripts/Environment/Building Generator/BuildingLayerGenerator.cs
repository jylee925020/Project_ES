using UnityEngine;

public class BuildingLayerGenerator : MonoBehaviour
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

    [Header("Ground")]
    [SerializeField] private SpriteRenderer groundPrefab;
    [SerializeField] private float groundHeight = 5f;

    private void Start()
    {
        GenerateGround();
        GenerateBuildings();
    }
    private void GenerateGround()
    {
        SpriteRenderer ground = Instantiate(
            groundPrefab,
            transform
        );

        float requiredWidth = horizontalRange + widthRange.y;

        Vector2 spriteSize = ground.sprite.bounds.size;

        ground.transform.localScale = new Vector3(
            requiredWidth / spriteSize.x,
            groundHeight / spriteSize.y,
            1f
        );

        ground.transform.localPosition = new Vector3(
            0f,
            -groundHeight * 0.5f,
            0f
        );
    }


    private void GenerateBuildings()
    {
        for (int i = 0; i < count; i++)
        {
            float width = Random.Range(widthRange.x, widthRange.y);
            float height = Random.Range(heightRange.x, heightRange.y);

            float x = Random.Range(
                -horizontalRange * 0.5f,
                 horizontalRange * 0.5f
            );

            SpriteRenderer rectangle = Instantiate(
                rectanglePrefab,
                transform
            );

            Vector2 spriteSize = rectangle.sprite.bounds.size;

            rectangle.transform.localScale = new Vector3(
                width / spriteSize.x,
                height / spriteSize.y,
                1f
            );

            rectangle.transform.localPosition = new Vector3(
                x,
                height * 0.5f,
                0f
            );

            byte n = (byte)Random.Range(minBrightness, maxBrightness + 1);
            rectangle.color = new Color32(n, n, n, 255);
        }
    }

    public void Configure(int buildingCount, float range)
    {
        count = buildingCount;
        horizontalRange = range;
    }


}