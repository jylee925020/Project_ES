#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[ExecuteAlways]
public class VFXSpawner : MonoBehaviour
{
    #region Inspector

    [SerializeField] private GameObject vfxPrefab;

    [Header("Transform")]
    [SerializeField] private Vector2 localPosition;
    [SerializeField] private float localAngle;
    [SerializeField] private Vector2 localScale = Vector2.one;

    [Header("Appearance")]
    [SerializeField] private Color tint = Color.white;

    [Header("Spawn")]
    [SerializeField] private bool spawnAsChild = true;

    [Header("Preview")]
    [SerializeField] private bool showPreview = true;

    #endregion

    #region Preview State

    private const string PreviewName = "[VFX Preview]";

    private GameObject previewInstance;
    private GameObject previewPrefab;
    private bool refreshScheduled;

    #endregion

    #region Runtime Spawn

    public GameObject Spawn()
    {
        if (vfxPrefab == null)
        {
            Debug.LogError(
                $"{name}: VFX 프리팹이 없습니다.",
                this
            );

            return null;
        }

        return spawnAsChild
            ? SpawnAsChild()
            : SpawnIndependent();
    }

    private GameObject SpawnAsChild()
    {
        GameObject instance = Instantiate(
            vfxPrefab,
            transform
        );

        ApplyLocalTransform(instance.transform);
        ApplyTint(instance);

        return instance;
    }

    private GameObject SpawnIndependent()
    {
        Vector3 worldPosition =
            transform.TransformPoint(localPosition);

        Quaternion worldRotation =
            GetWorldRotation(transform, localAngle);

        GameObject instance = Instantiate(
            vfxPrefab,
            worldPosition,
            worldRotation
        );

        Vector3 originScale = transform.lossyScale;

        instance.transform.localScale = new Vector3(
            originScale.x * localScale.x,
            originScale.y * localScale.y,
            1f
        );

        ApplyTint(instance);

        return instance;
    }

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        SchedulePreviewRefresh();
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SchedulePreviewRemoval();
        }
#endif
    }

    private void OnValidate()
    {
        SchedulePreviewRefresh();
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (previewInstance == null)
            return;

        ApplyLocalTransform(previewInstance.transform);
        ApplyTint(previewInstance);
    }

    #endregion

    #region Preview Scheduling

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

    #endregion

    #region Preview Management

    private void RefreshPreview()
    {
        if (Application.isPlaying)
            return;

        previewInstance ??= FindExistingPreview();

        if (!showPreview || vfxPrefab == null)
        {
            RemovePreview();
            return;
        }

        bool prefabChanged =
            previewInstance == null ||
            previewPrefab != vfxPrefab;

        if (prefabChanged)
        {
            RemovePreview();
            CreatePreview();
        }

        if (previewInstance == null)
            return;

        ApplyLocalTransform(previewInstance.transform);
        ApplyTint(previewInstance);
    }

    private void CreatePreview()
    {
        if (vfxPrefab == null)
            return;

#if UNITY_EDITOR
        previewInstance = PrefabUtility.InstantiatePrefab(
            vfxPrefab,
            transform
        ) as GameObject;
#else
        previewInstance = Instantiate(
            vfxPrefab,
            transform
        );
#endif

        if (previewInstance == null)
            return;

        previewInstance.name = PreviewName;

        previewInstance.hideFlags =
            HideFlags.DontSaveInEditor |
            HideFlags.DontSaveInBuild |
            HideFlags.NotEditable;

        previewPrefab = vfxPrefab;

        DisablePreviewBehaviours(previewInstance);
        ApplyLocalTransform(previewInstance.transform);
        ApplyTint(previewInstance);
    }

    private void RemovePreview()
    {
        previewInstance ??= FindExistingPreview();

        if (previewInstance == null)
            return;

        if (Application.isPlaying)
        {
            Destroy(previewInstance);
        }
        else
        {
            DestroyImmediate(previewInstance);
        }

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

    private static void DisablePreviewBehaviours(
        GameObject preview)
    {
        foreach (MonoBehaviour behaviour in
                 preview.GetComponentsInChildren<MonoBehaviour>(true))
        {
            behaviour.enabled = false;
        }

        foreach (Animator animator in
                 preview.GetComponentsInChildren<Animator>(true))
        {
            animator.enabled = false;
        }

        foreach (ParticleSystem particleSystem in
                 preview.GetComponentsInChildren<ParticleSystem>(true))
        {
            particleSystem.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    #endregion

    #region Appearance And Transform

    private void ApplyLocalTransform(Transform target)
    {
        target.localPosition = localPosition;

        target.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                localAngle
            );

        target.localScale = new Vector3(
            localScale.x,
            localScale.y,
            1f
        );
    }

    private void ApplyTint(GameObject target)
    {
        AttackVFX[] vfxComponents =
            target.GetComponentsInChildren<AttackVFX>(true);

        foreach (AttackVFX vfx in vfxComponents)
        {
            SpriteRenderer renderer =
                vfx.GetComponentInChildren<SpriteRenderer>(true);

            if (renderer == null)
                continue;

            Color appliedColor = new Color(
                tint.r,
                tint.g,
                tint.b,
                renderer.color.a
            );

            vfx.SetColor(appliedColor);
        }

        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            AttackVFX owner =
                renderer.GetComponentInParent<AttackVFX>();

            if (owner != null)
                continue;

            renderer.color = new Color(
                tint.r,
                tint.g,
                tint.b,
                renderer.color.a
            );
        }
    }

    private static Quaternion GetWorldRotation(
        Transform origin,
        float localAngle)
    {
        Vector3 localDirection =
            Quaternion.Euler(0f, 0f, localAngle) *
            Vector3.right;

        Vector3 worldDirection =
            origin.localToWorldMatrix.MultiplyVector(
                localDirection
            );

        float worldAngle = Mathf.Atan2(
            worldDirection.y,
            worldDirection.x
        ) * Mathf.Rad2Deg;

        return Quaternion.Euler(
            0f,
            0f,
            worldAngle
        );
    }

    #endregion
}