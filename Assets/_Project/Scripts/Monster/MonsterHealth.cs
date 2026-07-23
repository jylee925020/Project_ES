using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    public int MaxHealth => maxHealth;
    [SerializeField] private MonsterAI monsterAI;
    [SerializeField] private MonsterMovement monsterMovement;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        Debug.Log($"{name} 피격: {damage}, 남은 체력: {CurrentHealth}");

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;

        monsterAI.SetPatrolEnabled(false);
        monsterMovement.Stop();

        gameObject.SetActive(false);
    }
}