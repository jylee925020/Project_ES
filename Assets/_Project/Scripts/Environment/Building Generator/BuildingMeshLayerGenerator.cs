using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BuildingMeshLayerGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private int count = 20;
    [SerializeField] private float horizontalRange = 20f;

    [Header("Rectangle Size")]
    [SerializeField] private Vector2 widthRange = new(1f, 4f);
    [SerializeField] private Vector2 heightRange = new(2f, 8f);

    [Header("Color")]
    [SerializeField, Range(0, 255)] private int minBrightness = 0;
    [SerializeField, Range(0, 255)] private int maxBrightness = 100;

    [Header("Ground")]
    [SerializeField] private float groundHeight = 5f;
    [SerializeField] private Color groundColor = Color.black;

    [Header("Position")]
    [SerializeField] private float yOffset = 0f;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        mesh = new Mesh
        {
            name = "Building Layer Mesh"
        };

        meshRenderer = GetComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer.material = buildingMaterial;
    }

    private void Start()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Color> colors = new();

        for (int i = 0; i < count; i++)
        {
            float width = Random.Range(widthRange.x, widthRange.y);
            float height = Random.Range(heightRange.x, heightRange.y);

            float x = Random.Range(
                -horizontalRange * 0.5f,
                 horizontalRange * 0.5f
            );

            byte n = (byte)Random.Range(
                minBrightness,
                maxBrightness + 1
            );

            Color32 color = new(n, n, n, 255);

            AddRectangle(
                vertices,
                triangles,
                colors,
                x,
                width,
                height,
                color,
                yOffset
            );
        }

        AddRectangle(
            vertices,
            triangles,
            colors,
            0f,
            horizontalRange + widthRange.y,
            groundHeight,
            groundColor,
            -groundHeight + yOffset
        );

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.RecalculateBounds();
    }

    private static void AddRectangle(
        List<Vector3> vertices,
        List<int> triangles,
        List<Color> colors,
        float x,
        float width,
        float height,
        Color color,
        float bottom)
    {
        int startIndex = vertices.Count;

        float left = x - width * 0.5f;
        float right = x + width * 0.5f;
        float top = bottom + height;

        vertices.Add(new Vector3(left, bottom, 0f));
        vertices.Add(new Vector3(left, top, 0f));
        vertices.Add(new Vector3(right, top, 0f));
        vertices.Add(new Vector3(right, bottom, 0f));

        triangles.Add(startIndex);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 3);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    public void Configure(int buildingCount, float range)
    {
        count = buildingCount;
        horizontalRange = range;
    }

    public void SetSorting(string sortingLayerName, int order)
    {
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = order;
    }

    public void SetFogAmount(float amount)
    {
        meshRenderer.material.SetFloat("_FogAmount", amount);
    }
}