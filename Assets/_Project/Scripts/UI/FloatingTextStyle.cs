using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FloatingTextStyle : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private Material materialInstance;


    [Header("Face")]
    [SerializeField] private Color faceColor = Color.white;

    [Header("Outline")]
    [SerializeField] private Color outlineColor = Color.black;
    [Range(0f, 1f)]
    [SerializeField] private float outlineWidth = 0.2f;

    [Header("Underlay")]
    [SerializeField] private bool underlayEnabled = true;
    [SerializeField] private Color underlayColor = new Color(0f, 0f, 0f, 0.5f);
    [Range(-1f, 1f)]
    [SerializeField] private float underlayOffsetX = 0.2f;
    [Range(-1f, 1f)]
    [SerializeField] private float underlayOffsetY = -0.2f;
    [Range(-1f, 1f)]
    [SerializeField] private float underlayDilate;
    [Range(0f, 1f)]
    [SerializeField] private float underlaySoftness;

    private void Awake()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }

    private void Apply()
    {
        if (text == null)
            return;

        text.color = faceColor;

        if (!Application.isPlaying)
            return;

        if (materialInstance == null)
        {
            materialInstance = new Material(text.fontSharedMaterial);
            text.fontSharedMaterial = materialInstance;
        }

        materialInstance.SetColor("_OutlineColor", outlineColor);
        materialInstance.SetFloat("_OutlineWidth", outlineWidth);

        if (underlayEnabled)
        {
            materialInstance.EnableKeyword("UNDERLAY_ON");
            materialInstance.SetColor("_UnderlayColor", underlayColor);
            materialInstance.SetFloat("_UnderlayOffsetX", underlayOffsetX);
            materialInstance.SetFloat("_UnderlayOffsetY", underlayOffsetY);
            materialInstance.SetFloat("_UnderlayDilate", underlayDilate);
            materialInstance.SetFloat("_UnderlaySoftness", underlaySoftness);
        }
        else
        {
            materialInstance.DisableKeyword("UNDERLAY_ON");
        }
    }
}