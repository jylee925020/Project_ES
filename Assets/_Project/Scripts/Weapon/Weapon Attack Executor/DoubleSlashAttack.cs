using System.Collections;
using UnityEngine;

public class DoubleSlashAttack : WeaponAttackExecutor
{
    [Header("Timing")]
    [SerializeField, Min(0f)] private float firstStartupTime = 0.12f;
    [SerializeField, Min(0f)] private float firstActiveTime = 0.05f;

    [SerializeField, Min(0f)] private float intervalTime = 0.1f;

    [SerializeField, Min(0f)] private float secondActiveTime = 0.05f;
    [SerializeField, Min(0f)] private float recoveryTime = 0.28f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;

    [SerializeField] private BoxHitBoxSpawner firstHitBoxSpawner;
    [SerializeField] private BoxHitBoxSpawner secondHitBoxSpawner;

    [Header("VFX")]
    [SerializeField] private SimpleVFXSpawner firstVFXSpawner;
    [SerializeField] private SimpleVFXSpawner secondVFXSpawner;

    private AttackHitBox currentHitBox;

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(firstStartupTime);

        SpawnSlash(firstHitBoxSpawner, firstVFXSpawner);

        yield return new WaitForSeconds(firstActiveTime);

        DisableCurrentHitBox();

        yield return new WaitForSeconds(intervalTime);

        SpawnSlash(secondHitBoxSpawner, secondVFXSpawner);

        yield return new WaitForSeconds(secondActiveTime);

        DisableCurrentHitBox();

        yield return new WaitForSeconds(recoveryTime);
    }

    public override void Interrupt()
    {
        DisableCurrentHitBox();
    }

    private void SpawnSlash(
        BoxHitBoxSpawner hitBoxSpawner,
        SimpleVFXSpawner vfxSpawner)
    {
        if (hitBoxSpawner != null)
        {
            currentHitBox = hitBoxSpawner.Spawn(
                damage,
                AttackFaction.Player
            );
        }

        if (vfxSpawner != null)
            vfxSpawner.Spawn();
    }

    private void DisableCurrentHitBox()
    {
        if (currentHitBox == null)
            return;

        currentHitBox.DisableDamage();
        currentHitBox = null;
    }
}