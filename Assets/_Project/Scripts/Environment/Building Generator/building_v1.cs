using System.Collections.Generic;
using UnityEngine;

public class BuildingBackgroundGenerator : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] private BuildingLayerGenerator layerPrefab;
    [SerializeField] private int layerCount = 5;

    [Header("Distance")]
    [SerializeField] private float referenceDistance = 10f;
    [SerializeField] private float firstLayerDistance = 10f;
    [SerializeField] private float layerDistanceInterval = 10f;

    [Header("Base Generation")]
    [SerializeField] private float baseHorizontalRange = 50f;
    [SerializeField] private int baseBuildingCount = 20;

    [Header("Parallax")]
    [SerializeField] private Transform cameraTransform;

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

            // 멀수록 카메라를 많이 따라가므로
            // 화면상 이동량은 perspectiveScale에 비례해 작아진다.
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

            BuildingLayerGenerator layer = Instantiate(
                layerPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            layer.Configure(buildingCount, horizontalRange);

            Transform layerTransform = layer.transform;

            layerTransform.localScale *= perspectiveScale;

            layerTransforms.Add(layerTransform);
            initialLayerPositions.Add(layerTransform.position);
            perspectiveScales.Add(perspectiveScale);
        }
    }
}