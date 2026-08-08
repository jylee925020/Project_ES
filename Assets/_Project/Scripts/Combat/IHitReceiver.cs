using System;

/// <summary>
/// 공격을 받을 수 있는 대상이 구현하는 인터페이스.
/// 공격자는 피격 대상의 구체적인 내부 구조를 알 필요 없이
/// ReceiveHit을 통해 공격 정보를 전달한다.
/// </summary>
public interface IHitReceiver
{
    event Action<HitInfo> OnHit;
    void ReceiveHit(HitInfo hitInfo);
}