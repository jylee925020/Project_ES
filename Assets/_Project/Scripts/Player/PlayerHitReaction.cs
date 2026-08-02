using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어가 피해를 받았을 때 발생하는 피격 경직을 관리한다.
/// 피격 상태 자체는 PlayerState가 소유하고,
/// 이 클래스는 경직 시작과 종료 시점만 관리한다.
/// </summary>
public class PlayerHitReaction : MonoBehaviour
{
    private PlayerState state;
    private PlayerPhysics physics;

    [Header("Hit Stun")]
    [SerializeField] private float hitStunDuration = 0.2f;

    [Header("Hit Knockback")]
    [SerializeField] private float knockbackPower = 8f;
    [SerializeField] private Vector2 knockbackDirection = Vector2.up + Vector2.left;

    private Coroutine hitStunRoutine;

    private void Awake()
    {
        state = GetComponent<PlayerState>();
        physics = GetComponent<PlayerPhysics>();
    }

    public void ApplyKnockback()
    {
        Vector2 direction = knockbackDirection.normalized;

        if (!state.IsFacingRight)
            direction.x = -direction.x;

        physics.SetVelocity(direction * knockbackPower);
    }

    public void ApplyHitStun()
    {
        if (state.IsDead)
            return;

        if (hitStunRoutine != null)
            StopCoroutine(hitStunRoutine);

        state.BeginAction(PlayerActionType.HitStun);
        hitStunRoutine = StartCoroutine(HitStunRoutine());
    }

    private IEnumerator HitStunRoutine()
    {
        yield return new WaitForSeconds(hitStunDuration);

        state.EndAction(PlayerActionType.HitStun);

        hitStunRoutine = null;
    }

    public void CancelHitStun()
    {
        if (hitStunRoutine != null)
        {
            StopCoroutine(hitStunRoutine);
            hitStunRoutine = null;
        }

        state.EndAction(PlayerActionType.HitStun);
    }
}