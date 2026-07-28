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

    public void ApplyHitStun()
    {
        if (state.IsDead)
            return;

        // 또 맞으면 기존 코루틴 지우고 다시 실행.
        if (hitStunRoutine != null)
        {
            StopCoroutine(hitStunRoutine);
        }
        state.BeginAction(PlayerActionType.HitStun);

        ApplyHitKnockback();    // 피격 넉백

        hitStunRoutine = StartCoroutine(HitStunRoutine());
    }

    private void ApplyHitKnockback()
    {
        Vector2 dir = knockbackDirection.normalized;
        if (!state.IsFacingRight) dir.x = -dir.x;

        physics.SetVelocity(dir *  knockbackPower);
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