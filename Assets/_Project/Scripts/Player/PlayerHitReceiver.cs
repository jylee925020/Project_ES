using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// 플레이어가 외부 공격을 받는 진입점.
/// 피격 가능 여부를 검사하고 피해, 사망, 피격 반응을 순서대로 처리한다.
/// 피격시 무적 시간을 설정하고 적용한다.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerState))]
[RequireComponent(typeof(PlayerHitReaction))]
public class PlayerHitReceiver : MonoBehaviour, IHitReceiver
{
    private PlayerHealth health;
    private PlayerState state;
    private PlayerHitReaction hitReaction;

    [Header("피격 무적시간")]
    [SerializeField] private float invincibilityDuration = 0.5f;

    private bool isInvincible;
    private Coroutine invincibilityRoutine;

    // 이벤트: 무적 시작 및 종료
    public event Action OnInvincibilityStarted;
    public event Action OnInvincibilityEnded;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        state = GetComponent<PlayerState>();
        hitReaction = GetComponent<PlayerHitReaction>();
    }

    public void ReceiveHit(HitInfo hitInfo)
    {
        if (hitInfo.Faction != AttackFaction.Monster)
            return;

        if (state.IsDead || isInvincible)
            return;

        health.TakeDamage(hitInfo.Damage);

        hitReaction.ApplyKnockback();

        if (health.IsDead)
        {
            HandleDeath();
            return;
        }

        hitReaction.ApplyHitStun();
        BeginInvincibility();
    }

    private void BeginInvincibility()
    {
        if (invincibilityRoutine != null)
            StopCoroutine(invincibilityRoutine);

        invincibilityRoutine = StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        OnInvincibilityStarted?.Invoke();

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        invincibilityRoutine = null;

        OnInvincibilityEnded?.Invoke();
    }

    private void HandleDeath()
    {
        state.SetDead();

        Debug.Log($"{name} 사망");
    }
}