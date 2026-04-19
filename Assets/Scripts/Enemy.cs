using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int goldReward = 5;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float playerDetectionRange = 4f;

    private int currentHealth;
    private Rigidbody2D rb;
    private Tower tower;
    private Transform player;
    private PlayerHealth playerHealth;
    private int moveDirection;
    private float attackTimer;
    private bool isAttacking;
    private Transform currentContactTarget;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject towerObj = GameObject.FindGameObjectWithTag("Tower");
        if (towerObj != null) tower = towerObj.GetComponent<Tower>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    private Transform GetTarget()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) < playerDetectionRange)
            return player;
        return tower != null ? tower.transform : null;
    }

    private void FixedUpdate()
    {
        Transform target = GetTarget();
        if (target != null)
            moveDirection = target.position.x > transform.position.x ? 1 : -1;

        Vector2 v = rb.linearVelocity;
        v.x = isAttacking ? 0f : moveDirection * moveSpeed;
        rb.linearVelocity = v;
    }

    private void Update()
    {
        if (!isAttacking || currentContactTarget == null) return;
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            DealDamage();
            attackTimer = attackRate;
        }
    }

    private void DealDamage()
    {
        if (currentContactTarget == null) return;
        if (currentContactTarget.CompareTag("Tower") && tower != null)
            tower.TakeDamage(attackDamage);
        else if (currentContactTarget.CompareTag("Player") && playerHealth != null)
            playerHealth.TakeDamage(attackDamage, transform.position);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Tower") || col.collider.CompareTag("Player"))
        {
            isAttacking = true;
            currentContactTarget = col.collider.transform;
            attackTimer = attackRate * 0.5f;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.transform == currentContactTarget)
        {
            isAttacking = false;
            currentContactTarget = null;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.AddGold(goldReward);
        Destroy(gameObject);
    }
}