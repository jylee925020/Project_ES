using UnityEngine;
using System.Collections;

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
        animator.ResetTrigger(HitHash);
        animator.SetTrigger(DieHash);
    }

    public void DisableMonster()
    {
        Debug.Log($"{name}: DisableMonster");
        gameObject.SetActive(false);
    }
}