using UnityEngine;

/// <summary>
/// 지정 위치에서 바라보는 방향으로 Raycast를 수행하고
/// 처음 맞은 유효한 대상에게 HitInfo를 전달한다.
/// </summary>
public class RayHitBoxSpawner : MonoBehaviour
{
    [Header("Ray")]
    [SerializeField, Min(0f)] private float distance = 10f;
    [SerializeField] private LayerMask targetLayer;

    [Header("Origin")]
    [SerializeField] private Vector2 localOffset;

    [Header("Gizmo")]
    [SerializeField]
    private Color gizmoColor = Color.red;

    public RayHitResult Fire(int damage, AttackFaction faction)
    {
        Vector2 origin = GetWorldOrigin();
        Vector2 direction = GetWorldDirection();

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            targetLayer
        );

        Vector2 endPoint = hit.collider != null
            ? hit.point
            : origin + direction * distance;

        if (hit.collider != null)
        {
            IHitReceiver receiver =
                hit.collider.GetComponentInParent<IHitReceiver>();

            if (receiver != null)
            {
                HitInfo hitInfo = new HitInfo(
                    damage,
                    faction
                );

                receiver.ReceiveHit(hitInfo);
            }
        }

        return new RayHitResult(
            origin,
            endPoint,
            hit
        );
    }

    private Vector2 GetWorldOrigin()
    {
        float facingSign = transform.lossyScale.x >= 0f ? 1f : -1f;

        Vector3 offset = new Vector3(
            localOffset.x * facingSign,
            localOffset.y,
            0f
        );

        return transform.position + offset;
    }

    private Vector2 GetWorldDirection()
    {
        return transform.lossyScale.x >= 0f
            ? Vector2.right
            : Vector2.left;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector2 origin = GetWorldOrigin();
        Vector2 direction = GetWorldDirection();

        Gizmos.DrawLine(
            origin,
            origin + direction * distance
        );
    }
}