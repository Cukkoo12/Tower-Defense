using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private float attackAnimDuration = 0.35f;
    [SerializeField] private float attackDelay = 0.15f;

    private PlayerController controller;
    private float attackTimer;

    private void Awake() => controller = GetComponent<PlayerController>();

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)) && attackTimer <= 0f)
        {
            StartAttack();
            attackTimer = attackCooldown;
        }
    }

    private void StartAttack()
    {
        controller.PlayState("Attack");
        controller.LockAnimation(attackAnimDuration);
        Invoke(nameof(DealDamage), attackDelay);
    }

    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}