using UnityEngine;

/// <summary>
/// 몬스터가 외부 공격을 받는 진입점.
/// 전달받은 HitInfo를 몬스터 내부 컴포넌트에 분배한다.
/// </summary>
[RequireComponent(typeof(MonsterHealth))]
[RequireComponent(typeof(MonsterAnimation))]
public class MonsterHitReceiver : MonoBehaviour, IHitReceiver
{
    private MonsterHealth health;
    private MonsterAnimation animation;

    private void Awake()
    {
        health = GetComponent<MonsterHealth>();
        animation = GetComponent<MonsterAnimation>();
    }

    public void ReceiveHit(HitInfo hitInfo)
    {
        if (hitInfo.Faction != AttackFaction.Player)
            return;

        health.TakeDamage(hitInfo.Damage);

        if (!health.IsDead)
        {
            animation.PlayHit();
        }
    }
}