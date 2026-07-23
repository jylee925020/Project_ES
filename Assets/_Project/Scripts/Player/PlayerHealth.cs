using UnityEngine;
/// <summary>
/// 플레이어의 체력과 관련된 기능을 담당한다.
/// - 체력과 피해 처리
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
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

        Debug.Log(
            $"{name} 피격: {damage}, 남은 체력: {CurrentHealth}/{maxHealth}"
        );

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;

        Debug.Log($"{name} 사망");

        // 이후 입력 차단, 애니메이션, 리스폰 등을 연결한다.
    }
}