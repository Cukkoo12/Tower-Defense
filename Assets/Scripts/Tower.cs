using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private int maxHealth = 200;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Kule Can: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Kule yıkıldı! Oyun bitti.");
        Time.timeScale = 0f;
    }
}