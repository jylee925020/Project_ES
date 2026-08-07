using System;
using UnityEngine;

/// <summary>
/// 플레이어의 체력을 소유하고 변경한다.
/// 피격 반응과 사망 처리는 담당하지 않는다.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0;

    public event Action OnDied;
    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
            return;

        CurrentHealth =
            Mathf.Max(CurrentHealth - damage, 0);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        Debug.Log(
            $"{name} 피격: {damage}, " +
            $"남은 체력: {CurrentHealth}/{maxHealth}"
        );

        if (IsDead)
        {
            OnDied?.Invoke();
        }
    }

    public void RestoreFullHealth()
    {
        CurrentHealth = maxHealth;
    }
}