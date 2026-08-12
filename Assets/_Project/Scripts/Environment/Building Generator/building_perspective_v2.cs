using UnityEngine;

public class BuildingPerspectiveManager : MonoBehaviour
{
    [System.Serializable]
    private class Layer
    {
        public BuildingLayerGenerator generator;

        [Min(0.01f)]
        public float distance = 10f;
    }

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Layer[] layers;

    [Header("Perspective")]
    [SerializeField, Min(0.01f)] private float referenceDistance = 10f;

    private Vector3 initialCameraPosition;
    private Vector3[] initialLayerPositions;
    private Vector3[] initialLayerScales;
    private float[] perspectiveScales;

    private void Start()
    {
        initialCameraPosition = cameraTransform.position;

        initialLayerPositions = new Vector3[layers.Length];
        initialLayerScales = new Vector3[layers.Length];
        perspectiveScales = new float[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            Layer layer = layers[i];
            Transform layerTransform = layer.generator.transform;

            initialLayerPositions[i] = layerTransform.position;
            initialLayerScales[i] = layerTransform.localScale;

            float perspectiveScale = referenceDistance / layer.distance;
            perspectiveScales[i] = perspectiveScale;

            layerTransform.localScale =
                initialLayerScales[i] * perspectiveScale;
        }
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - initialCameraPosition;

        for (int i = 0; i < layers.Length; i++)
        {
            Transform layerTransform = layers[i].generator.transform;

            float followFactor = 1f - perspectiveScales[i];

            Vector3 origin = initialLayerPositions[i];

            layerTransform.position = new Vector3(
                origin.x + cameraDelta.x * followFactor,
                origin.y + cameraDelta.y * followFactor,
                origin.z
            );
        }
    }
}