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
    [SerializeField] private float hitAnimDuration = 0.25f;
    [SerializeField] private float deathAnimDuration = 0.8f;

    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private Tower tower;
    private Transform player;
    private PlayerHealth playerHealth;
    private int moveDirection;
    private float attackTimer;
    private bool isAttacking;
    private bool isDead;
    private bool isHit;
    private float hitTimer;
    private Transform currentContactTarget;
    private string currentAnimState = "";
    private bool animLocked;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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
        if (isDead) return;

        Transform target = GetTarget();
        if (target != null)
            moveDirection = target.position.x > transform.position.x ? 1 : -1;

        Vector2 v = rb.linearVelocity;
        v.x = (isAttacking || isHit) ? 0f : moveDirection * moveSpeed;
        rb.linearVelocity = v;

        if (sr != null && moveDirection != 0)
            sr.flipX = moveDirection < 0;
    }

    private void Update()
    {
        if (isDead) return;

        if (isHit)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0f) isHit = false;
        }

        UpdateAnimation();

        if (!isAttacking || currentContactTarget == null) return;
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            DealDamage();
            PlayState("SkeletonAttack");
            attackTimer = attackRate;
        }
    }

    private void UpdateAnimation()
    {
        if (animLocked) return;
        string state;
        if (isHit) state = "SkeletonHit";
        else if (isAttacking) state = "SkeletonAttack";
        else state = "SkeletonWalk";
        PlayState(state);
    }

    private void PlayState(string state)
    {
        if (state == currentAnimState) return;
        if (animator != null) animator.Play(state);
        currentAnimState = state;
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
        if (isDead) return;
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
        if (isDead) return;
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isHit = true;
            hitTimer = hitAnimDuration;
            PlayState("SkeletonHit");
        }
    }

    private void Die()
    {
        isDead = true;
        animLocked = true;
        PlayState("SkeletonDeath");
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (GameManager.Instance != null) GameManager.Instance.AddGold(goldReward);
        Destroy(gameObject, deathAnimDuration);
    }
}