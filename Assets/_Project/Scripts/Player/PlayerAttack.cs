using System;
using UnityEngine;

/// <summary>
/// 플레이어의 공격을 제어하는 클래스
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    public event Action<string> OnAttacked;

    public void Attack()
    {
        OnAttacked?.Invoke("Swing_1");  // 임시로 직접 입력
        // 추후 현재 무기로부터 모션 이름을 가져오도록 할 예정
    }
}