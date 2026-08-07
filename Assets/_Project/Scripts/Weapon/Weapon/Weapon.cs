using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 무기의 공격 실행기와 표현 데이터를 보유하고
/// 요청받은 공격을 실행한다.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private WeaponAttackExecutor attackExecutor;

    [Header("Animation")]
    [SerializeField] private AnimationData groundAttackAnimation;
    [SerializeField] private AnimationData airAttackAnimation;

    public event Action<AnimationData> OnAttackStarted;
    public AnimationData GetAttackAnimation(bool isGrounded)
    {
        return isGrounded
            ? groundAttackAnimation
            : airAttackAnimation;
    }

    public IEnumerator Use()
    {
        if (attackExecutor == null)
            yield break;

        yield return attackExecutor.Execute();
    }

    public void ForceInterrupt()
    {
        attackExecutor.Interrupt();
    }

}