/// <summary>
/// 공격이 피격 대상에게 전달하는 정보.
/// 현재는 피해량만 포함하며,
/// 넉백이나 경직 등이 필요해질 때 확장한다.
/// </summary>
public readonly struct HitInfo
{
    public int Damage { get; }
    public AttackFaction Faction { get; }

    public HitInfo(int damage, AttackFaction faction)
    {
        Damage = damage;
        Faction = faction;
    }
}