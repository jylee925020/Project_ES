using UnityEngine;

/// <summary>
/// 플레이어의 전투 입력을 적절한 전투 실행 객체에 전달한다.
/// 현재는 Q 슬롯에 PlayerAttack이 임시로 연결되어 있다.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    private const int CombatSlotCount = 4;

    private PlayerAttack temporaryAttack;

    private void Awake()
    {
        temporaryAttack = GetComponent<PlayerAttack>();
    }

    /// <summary>
    /// 지정한 전투 슬롯의 사용을 시도한다.
    /// 현재는 0번 슬롯만 사용할 수 있다.
    /// </summary>
    public bool TryUseSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= CombatSlotCount)
            return false;

        if (slotIndex != 0)
            return false;

        if (temporaryAttack == null)
            return false;

        return temporaryAttack.TryAttack();
    }

    /// <summary>
    /// 현재 실행 중인 전투 행동을 강제로 종료한다.
    /// </summary>
    public void ForceInterruptCurrentAction()
    {
        if (temporaryAttack == null)
            return;

        temporaryAttack.ForceInterrupt();
    }
}