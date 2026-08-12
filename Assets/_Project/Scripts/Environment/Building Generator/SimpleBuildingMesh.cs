using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SimpleBuildingMesh : MonoBehaviour
{
    [SerializeField] private Material material;

    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh
        {
            name = "Building Mesh"
        };

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

        BuildMesh();
    }

    private void BuildMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Color> colors = new();

        AddBuilding(vertices, triangles, colors,
            new Vector2(-4f, 0f),
            new Vector2(2f, 5f),
            Color.red);

        AddBuilding(vertices, triangles, colors,
            new Vector2(-1f, 0f),
            new Vector2(2.5f, 8f),
            Color.green);

        AddBuilding(vertices, triangles, colors,
            new Vector2(3f, 0f),
            new Vector2(3f, 4f),
            Color.blue);

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);

        mesh.RecalculateBounds();
    }

    private static void AddBuilding(
        List<Vector3> vertices,
        List<int> triangles,
        List<Color> colors,
        Vector2 position,
        Vector2 size,
        Color color)
    {
        int startIndex = vertices.Count;

        float left = position.x;
        float right = position.x + size.x;
        float bottom = position.y;
        float top = position.y + size.y;

        vertices.Add(new Vector3(left, bottom));
        vertices.Add(new Vector3(left, top));
        vertices.Add(new Vector3(right, top));
        vertices.Add(new Vector3(right, bottom));

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
}