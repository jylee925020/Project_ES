using UnityEngine;

/// <summary>
/// 사각형 AttackHitBox의 설정과 생성을 담당한다.
/// </summary>
public class BoxHitBoxSpawner : MonoBehaviour
{
    [Header("HitBox")]
    [SerializeField] private AttackHitBox hitBoxPrefab;
    [SerializeField] private Vector2 localPosition;
    [SerializeField] private Vector2 size = Vector2.one;
    [SerializeField] private float localAngle;

    [Header("Preview")]
    [SerializeField] private bool showPreview = true;
    [SerializeField]
    private Color previewColor =
        new Color(1f, 0f, 0f, 0.8f);

    public AttackHitBox Spawn(
        int damage,
        AttackFaction faction)
    {
        if (hitBoxPrefab == null)
        {
            Debug.LogError(
                $"{name}: AttackHitBox 프리팹이 없습니다.",
                this
            );

            return null;
        }

        Vector3 worldPosition =
            transform.TransformPoint(localPosition);

        Quaternion worldRotation =
            GetWorldRotation(transform, localAngle);

        return AttackHitBox.Spawn(
            hitBoxPrefab,
            worldPosition,
            worldRotation,
            size,
            damage,
            faction
        );
    }


    private void OnDrawGizmos()
    {
        if (!showPreview)
            return;

        Vector3 worldPosition =
            transform.TransformPoint(localPosition);

        Quaternion worldRotation =
            GetWorldRotation(transform, localAngle);

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = Matrix4x4.TRS(
            worldPosition,
            worldRotation,
            Vector3.one
        );

        Gizmos.color = previewColor;
        Gizmos.DrawWireCube(Vector3.zero, size);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
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

    private void OnValidate()
    {
        size.x = Mathf.Max(0f, size.x);
        size.y = Mathf.Max(0f, size.y);
    }
}