using UnityEngine;

/// <summary>
/// 플레이어가 외부 공격을 받는 진입점.
/// 공격자는 플레이어 내부 구조를 알 필요 없이
/// IHitReceiver를 통해 HitInfo만 전달한다.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerHitReceiver : MonoBehaviour, IHitReceiver
{
    private PlayerHealth health;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    public void ReceiveHit(HitInfo hitInfo)
    {
        if (hitInfo.Faction != AttackFaction.Monster)
            return;

        health.TakeDamage(hitInfo.Damage);
    }
}