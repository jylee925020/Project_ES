using System.Collections;
using UnityEngine;

/// <summary>
/// 무기 공격의 구체적인 실행 방식을 정의한다.
/// 히트박스, 투사체, VFX 등의 저수준 도구를 조합해
/// 하나의 공격 시퀀스를 수행한다.
/// </summary>
public abstract class WeaponAttackExecutor : MonoBehaviour
{
    /// <summary>
    /// 공격 실행 과정을 반환한다.
    /// Weapon이 이 코루틴을 실행하고 완료 시점을 관리한다.
    /// </summary>
    public abstract IEnumerator Execute();

    /// <summary>
    /// 공격이 강제로 중단될 때 실행 중인 판정 등을 정리한다.
    /// </summary>
    public abstract void Interrupt();
}