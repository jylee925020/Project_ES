using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MonsterAI))]
[RequireComponent(typeof(MonsterAttack))]
[RequireComponent(typeof(MonsterHealth))]
public class MonsterAnimation : MonoBehaviour
{
    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int IsAttackStartupHash =
        Animator.StringToHash("IsAttackStartup");

    private static readonly int IsAttackingHash =
        Animator.StringToHash("IsAttacking");

    private static readonly int HitHash =
        Animator.StringToHash("Hit");

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    private Animator animator;
    private MonsterAI ai;
    private MonsterAttack attack;
    private MonsterHealth health;

    private bool deathPlayed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ai = GetComponent<MonsterAI>();
        attack = GetComponent<MonsterAttack>();
        health = GetComponent<MonsterHealth>();
    }

    private void Update()
    {
        if (health.IsDead)
        {
            PlayDeath();
            return;
        }

        animator.SetBool(IsMovingHash, ai.ShouldMove);
        animator.SetBool(IsAttackStartupHash, attack.IsInStartup);
        animator.SetBool(IsAttackingHash, attack.IsActive);
    }

    public void PlayHit()
    {
        if (health.IsDead)
            return;

        animator.SetTrigger(HitHash);
    }

    private void PlayDeath()
    {
        if (deathPlayed)
            return;

        deathPlayed = true;

        animator.SetBool(IsMovingHash, false);
        animator.SetBool(IsAttackStartupHash, false);
        animator.SetBool(IsAttackingHash, false);

        animator.SetTrigger(DieHash);
    }

    // Die 클립 마지막 프레임의 Animation Event에서 호출
    public void DisableMonster()
    {
        gameObject.SetActive(false);
    }
}