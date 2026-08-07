using System.Collections;
using UnityEngine;

/// <summary>
/// 한 번의 검 휘두르기 공격을 실행한다.
/// </summary>
public class SwordSlashAttack : WeaponAttackExecutor
{
    [Header("Timing")]
    [SerializeField, Min(0f)] private float startupTime = 0.12f;
    [SerializeField, Min(0f)] private float activeTime = 0.05f;
    [SerializeField, Min(0f)] private float recoveryTime = 0.28f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private BoxHitBoxSpawner hitBoxSpawner;

    [Header("VFX")]
    [SerializeField] private SimpleVFXSpawner vfxSpawner;

    private AttackHitBox currentHitBox;

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(startupTime);

        currentHitBox = hitBoxSpawner.Spawn(
            damage,
            AttackFaction.Player
        );

        if (vfxSpawner != null)
            vfxSpawner.Spawn();

        yield return new WaitForSeconds(activeTime);

        DisableCurrentHitBox();

        yield return new WaitForSeconds(recoveryTime);
    }

    public override void Interrupt()
    {
        DisableCurrentHitBox();
    }

    private void DisableCurrentHitBox()
    {
        if (currentHitBox == null)
            return;

        currentHitBox.DisableDamage();
        currentHitBox = null;
    }
}