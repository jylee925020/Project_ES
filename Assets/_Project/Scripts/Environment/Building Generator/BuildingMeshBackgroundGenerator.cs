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
    [SerializeField] private float baseY = -20f;
    [SerializeField] private float horizonY = -10f;

    [Header("Base Generation")]
    [SerializeField] private float baseHorizontalRange = 50f;
    [SerializeField] private int baseBuildingCount = 20;

    [Header("Parallax")]
    [SerializeField] private Transform cameraTransform;

    [Header("Fog")]
    [SerializeField, Range(0f, 1f)] private float nearFogAmount = 0f;
    [SerializeField, Range(0f, 1f)] private float farFogAmount = 0.8f;

    [Header("Gizmo")]
    [SerializeField] private float gizmoWidth = 40f;

    private readonly List<Transform> layerTransforms = new();
    private readonly List<Vector3> initialLayerPositions = new();
    private readonly List<float> perspectiveScales = new();
    private readonly List<float> layerDistances = new();

    private Vector3 initialCameraPosition;

    private void Awake()
    {
        GenerateLayers();
        ResetParallax();
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta =
            cameraTransform.position - initialCameraPosition;

        for (int i = 0; i < layerTransforms.Count; i++)
        {
            float followFactor =
                1f - perspectiveScales[i];

            Vector3 origin =
                initialLayerPositions[i];

            layerTransforms[i].position = new Vector3(
                origin.x + cameraDelta.x * followFactor,
                origin.y + cameraDelta.y * followFactor,
                origin.z
            );
        }

        // 리셋 테스트
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    ResetParallax();
        //}
    }

    private void GenerateLayers()
    {
        // 먼 레이어부터 생성
        for (int i = layerCount - 1; i >= 0; i--)
        {
            float distance =
                firstLayerDistance
                + layerDistanceInterval * i;

            float perspectiveScale =
                referenceDistance / distance;

            float generationRatio =
                distance / referenceDistance;

            float horizontalRange =
                baseHorizontalRange * generationRatio;

            int buildingCount =
                Mathf.RoundToInt(
                    baseBuildingCount * generationRatio
                );

            BuildingMeshLayerGenerator layer = Instantiate(
                layerPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            layer.Configure(
                buildingCount,
                horizontalRange
            );

            layer.SetSorting("Background", -i);

            Transform layerTransform =
                layer.transform;

            layerTransform.localScale *=
                perspectiveScale;

            layerTransforms.Add(layerTransform);
            perspectiveScales.Add(perspectiveScale);
            layerDistances.Add(distance);

            float fogT = layerCount <= 1
                ? 0f
                : (float)i / (layerCount - 1);

            float fogAmount =
                Mathf.Lerp(
                    nearFogAmount,
                    farFogAmount,
                    fogT
                );

            layer.SetFogAmount(fogAmount);
        }
    }

    public void ResetParallax()
    {
        initialCameraPosition =
            cameraTransform.position;

        initialLayerPositions.Clear();

        for (int i = 0; i < layerTransforms.Count; i++)
        {
            float distance =
                layerDistances[i];

            float depthScale =
                firstLayerDistance / distance;

            float relativeY =
                horizonY
                + (baseY - horizonY) * depthScale;

            Vector3 position = new Vector3(
                cameraTransform.position.x,
                cameraTransform.position.y + relativeY,
                layerTransforms[i].position.z
            );

            layerTransforms[i].position = position;
            initialLayerPositions.Add(position);
        }
    }

    private void OnDrawGizmos()
    {
        if (cameraTransform == null)
            return;

        float centerX =
            cameraTransform.position.x;

        float cameraY =
            cameraTransform.position.y;

        Vector3 baseLeft = new(
            centerX - gizmoWidth * 0.5f,
            cameraY + baseY,
            0f
        );

        Vector3 baseRight = new(
            centerX + gizmoWidth * 0.5f,
            cameraY + baseY,
            0f
        );

        Vector3 horizonLeft = new(
            centerX - gizmoWidth * 0.5f,
            cameraY + horizonY,
            0f
        );

        Vector3 horizonRight = new(
            centerX + gizmoWidth * 0.5f,
            cameraY + horizonY,
            0f
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(baseLeft, baseRight);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(horizonLeft, horizonRight);
    }
}