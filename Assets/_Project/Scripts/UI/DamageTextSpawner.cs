#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[ExecuteAlways]
public class DamageTextSpawner : MonoBehaviour
{
    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Preview")]
    [SerializeField] private bool showPreview = true;
    [SerializeField] private int previewDamage = 10;

    private const string PreviewName = "[Damage Text Preview]";

    private IHitReceiver hitReceiver;

    private GameObject previewInstance;
    private DamageText previewPrefab;
    private bool refreshScheduled;

    private void Awake()
    {
        if (Application.isPlaying)
            hitReceiver = GetComponent<IHitReceiver>();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            hitReceiver.OnHit += SpawnDamageText;
            return;
        }

        SchedulePreviewRefresh();
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            if (hitReceiver != null)
                hitReceiver.OnHit -= SpawnDamageText;

            return;
        }

#if UNITY_EDITOR
        SchedulePreviewRemoval();
#endif
    }

    private void OnValidate()
    {
        SchedulePreviewRefresh();
    }

    private void Update()
    {
        if (Application.isPlaying || previewInstance == null)
            return;

        previewInstance.transform.localPosition = spawnOffset;
    }

    private void SpawnDamageText(HitInfo hitInfo)
    {
        DamageText damageText = Instantiate(
            damageTextPrefab,
            transform.position + spawnOffset,
            Quaternion.identity
        );

        damageText.Initialize(hitInfo.Damage);
    }

    private void SchedulePreviewRefresh()
    {
#if UNITY_EDITOR
        if (Application.isPlaying || refreshScheduled)
            return;

        refreshScheduled = true;

        EditorApplication.delayCall += () =>
        {
            refreshScheduled = false;

            if (this == null)
                return;

            RefreshPreview();
        };
#endif
    }

    private void SchedulePreviewRemoval()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null)
                return;

            RemovePreview();
        };
#endif
    }

    private void RefreshPreview()
    {
        if (Application.isPlaying)
            return;

        previewInstance ??= FindExistingPreview();

        if (!showPreview || damageTextPrefab == null)
        {
            RemovePreview();
            return;
        }

        bool prefabChanged =
            previewInstance == null ||
            previewPrefab != damageTextPrefab;

        if (prefabChanged)
        {
            RemovePreview();
            CreatePreview();
        }

        if (previewInstance == null)
            return;

        previewInstance.transform.localPosition = spawnOffset;

        DamageText damageText =
            previewInstance.GetComponent<DamageText>();

        if (damageText != null)
            damageText.Initialize(previewDamage);
    }

    private void CreatePreview()
    {
        if (damageTextPrefab == null)
            return;

#if UNITY_EDITOR
        previewInstance = PrefabUtility.InstantiatePrefab(
            damageTextPrefab.gameObject,
            transform
        ) as GameObject;
#endif

        if (previewInstance == null)
            return;

        previewInstance.name = PreviewName;

        previewInstance.hideFlags =
            HideFlags.DontSaveInEditor |
            HideFlags.DontSaveInBuild |
            HideFlags.NotEditable;

        previewPrefab = damageTextPrefab;

        previewInstance.transform.localPosition = spawnOffset;

        DamageText damageText =
            previewInstance.GetComponent<DamageText>();

        if (damageText != null)
        {
            damageText.Initialize(previewDamage);
            damageText.enabled = false; // Disable the DamageText script in the preview
        }
    }

    private void RemovePreview()
    {
        previewInstance ??= FindExistingPreview();

        if (previewInstance == null)
            return;

        DestroyImmediate(previewInstance);

        previewInstance = null;
        previewPrefab = null;
    }

    private GameObject FindExistingPreview()
    {
        Transform child = transform.Find(PreviewName);

        return child != null
            ? child.gameObject
            : null;
    }
}