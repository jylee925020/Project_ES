using System.Collections;
using UnityEngine;

public class GunShotAttack : WeaponAttackExecutor
{
    [Header("Timing")]
    [SerializeField, Min(0f)] private float startupTime = 0.1f;
    [SerializeField, Min(0f)] private float recoveryTime = 0.2f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private RayHitBoxSpawner rayHitBoxSpawner;

    [Header("VFX")]
    [SerializeField] private SimpleVFXSpawner muzzleVFXSpawner;
    [SerializeField] private BulletTrailVFXSpawner bulletTrailSpawner;

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(startupTime);

        RayHitResult result = rayHitBoxSpawner.Fire(
            damage,
            AttackFaction.Player
        );

        if (bulletTrailSpawner != null)
        {
            bulletTrailSpawner.Spawn(
                result.Origin,
                result.EndPoint
            );
        }

        if (muzzleVFXSpawner != null)
            muzzleVFXSpawner.Spawn();

        yield return new WaitForSeconds(recoveryTime);
    }

    public override void Interrupt()
    {
    }
}