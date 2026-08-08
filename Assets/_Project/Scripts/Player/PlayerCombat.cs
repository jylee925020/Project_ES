using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 전투 상태와 무기 슬롯을 관리한다.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    private const int CombatSlotCount = 4;

    [SerializeField]
    private Weapon[] weapons = new Weapon[CombatSlotCount];

    private PlayerState state;

    private Weapon currentWeapon;
    private Coroutine combatRoutine;

    public event Action<AnimationData> OnAttackStarted;

    private void Awake()
    {
        state = GetComponent<PlayerState>();
    }

    private void OnEnable()
    {
        state.OnActionChanged += HandleActionChanged;
    }

    private void OnDisable()
    {
        state.OnActionChanged -= HandleActionChanged;
    }

    public bool TryUseSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weapons.Length)
            return false;

        if (!state.CanAttack)
            return false;

        Weapon weapon = weapons[slotIndex];

        if (weapon == null)
            return false;

        bool isGrounded = state.IsGrounded;

        state.BeginAction(PlayerActionType.Attack);

        currentWeapon = weapon;

        AnimationData animationData =
            weapon.GetAttackAnimation(isGrounded);

        if (animationData != null)
            OnAttackStarted?.Invoke(animationData);

        combatRoutine = StartCoroutine(
            UseWeaponRoutine(weapon)
        );


        return true;
    }
    private IEnumerator UseWeaponRoutine(Weapon weapon)
    {
        yield return weapon.Use();

        FinishCurrentAction();
    }

    public void ForceInterruptCurrentAction()
    {
        if (combatRoutine == null)
            return;

        StopCoroutine(combatRoutine);

        currentWeapon?.ForceInterrupt();

        FinishCurrentAction();
    }

    private void FinishCurrentAction()
    {
        if (state.CurrentAction == PlayerActionType.Attack)
            state.EndAction(PlayerActionType.Attack);

        currentWeapon = null;
        combatRoutine = null;
    }

    private void HandleActionChanged(PlayerActionType action)
    {
        if (action == PlayerActionType.HitStun ||
            action == PlayerActionType.Dead)
        {
            ForceInterruptCurrentAction();
        }
    }
}