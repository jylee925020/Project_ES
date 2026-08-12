using System.Collections.Generic;
using UnityEngine;

public class BuildingMeshBackgroundGenerator : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] private BuildingMeshLayerGenerator layerPrefab;
    [SerializeField] private int layerCount = 5;

    [Header("Distance")]
    [SerializeField] private float referenceDistance = 10f;
    [SerializeField] private float firstLayerDistance = 10f;
    [SerializeField] private float layerDistanceInterval = 10f;

    [Header("Perspective")]
    [SerializeField] private float horizonY = 0f;

    [Header("Base Generation")]
    [SerializeField] private float baseHorizontalRange = 50f;
    [SerializeField] private int baseBuildingCount = 20;

    [Header("Parallax")]
    [SerializeField] private Transform cameraTransform;

    [Header("Fog")]
    [SerializeField, Range(0f, 1f)] private float nearFogAmount = 0f;
    [SerializeField, Range(0f, 1f)] private float farFogAmount = 0.8f;

    private readonly List<Transform> layerTransforms = new();
    private readonly List<Vector3> initialLayerPositions = new();
    private readonly List<float> perspectiveScales = new();

    private Vector3 initialCameraPosition;

    private void Awake()
    {
        initialCameraPosition = cameraTransform.position;

        GenerateLayers();
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta =
            cameraTransform.position - initialCameraPosition;

        for (int i = 0; i < layerTransforms.Count; i++)
        {
            float perspectiveScale = perspectiveScales[i];
            float followFactor = 1f - perspectiveScale;

            Vector3 origin = initialLayerPositions[i];

            layerTransforms[i].position = new Vector3(
                origin.x + cameraDelta.x * followFactor,
                origin.y + cameraDelta.y * followFactor,
                origin.z
            );
        }
    }

    private void GenerateLayers()
    {
        // 먼 레이어부터 생성
        for (int i = layerCount - 1; i >= 0; i--)
        {
            float distance =
                firstLayerDistance + layerDistanceInterval * i;

            float perspectiveScale =
                referenceDistance / distance;

            float generationRatio =
                distance / referenceDistance;

            float horizontalRange =
                baseHorizontalRange * generationRatio;

            int buildingCount =
                Mathf.RoundToInt(baseBuildingCount * generationRatio);

            BuildingMeshLayerGenerator layer = Instantiate(
                layerPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            layer.Configure(buildingCount, horizontalRange);
            layer.SetSortingOrder(-i);

            Transform layerTransform = layer.transform;

            layerTransform.localScale *= perspectiveScale;

            float baseY = transform.position.y;

            float perspectiveY =
                horizonY
                + (baseY - horizonY) * perspectiveScale;

            layerTransform.position = new Vector3(
                layerTransform.position.x,
                perspectiveY,
                layerTransform.position.z
            );

            layerTransforms.Add(layerTransform);
            initialLayerPositions.Add(layerTransform.position);
            perspectiveScales.Add(perspectiveScale);

            float fogT = layerCount <= 1
                ? 0f
                : (float)i / (layerCount - 1);

            float fogAmount =
                Mathf.Lerp(nearFogAmount, farFogAmount, fogT);

            layer.SetFogAmount(fogAmount);
        }
    }
}