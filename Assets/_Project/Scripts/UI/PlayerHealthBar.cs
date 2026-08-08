using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;


    private void Start()
    {
        playerHealth = ObjectManager.Instance.PlayerHealth;

        UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);

        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        fillImage.fillAmount = currentHealth / (float)maxHealth;
    }
}