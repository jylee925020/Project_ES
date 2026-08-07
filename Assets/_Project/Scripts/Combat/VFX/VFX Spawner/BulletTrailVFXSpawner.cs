using UnityEngine;

public class BulletTrailVFXSpawner : MonoBehaviour
{
    [SerializeField] private BulletTrailVFX trailPrefab;

    [Header("Appearance")]
    [SerializeField] private Color color = Color.white;
    [SerializeField, Min(0f)] private float thickness = 0.05f;

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float lifetime = 0.05f;

    public void Spawn(Vector2 start, Vector2 end)
    {
        if (trailPrefab == null)
            return;

        BulletTrailVFX trail = Instantiate(
            trailPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        trail.Initialize(
            start,
            end,
            color,
            thickness,
            lifetime
        );
    }
}