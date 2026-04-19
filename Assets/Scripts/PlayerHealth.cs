using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 0.6f;
    [SerializeField] private float knockbackForce = 7f;
    [SerializeField] private float hitAnimDuration = 0.3f;

    private int currentHealth;
    private float invulTimer;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerController controller;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        invulTimer -= Time.deltaTime;
        if (sr != null && !isDead)
            sr.color = invulTimer > 0f ? new Color(1f, 0.4f, 0.4f, 0.7f) : Color.white;
    }

    public void TakeDamage(int amount, Vector2 fromPosition)
    {
        if (invulTimer > 0f || isDead) return;
        currentHealth -= amount;
        invulTimer = invulnerabilityTime;

        Vector2 dir = ((Vector2)transform.position - fromPosition).normalized;
        if (rb != null)
            rb.linearVelocity = new Vector2(dir.x * knockbackForce, knockbackForce * 0.5f);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            controller.PlayState("Hit");
            controller.LockAnimation(hitAnimDuration);
        }
    }

    private void Die()
    {
        isDead = true;
        controller.PlayState("Death");
        controller.LockAnimation(999f);
        controller.enabled = false;
        Time.timeScale = 0f;
    }
}