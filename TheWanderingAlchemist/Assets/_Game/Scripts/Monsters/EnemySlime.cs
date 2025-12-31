using UnityEngine;

public class EnemySlime : EnemyAI
{
    [Header("Slime Settings")]
    [SerializeField] private int touchDamage = 10;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        ApplyDamage(collision);
        ApplyKnockback(collision);
    }

    // =======================
    // DAMAGE
    // =======================
    private void ApplyDamage(Collision2D collision)
    {
        PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(touchDamage);
        }
    }

    // =======================
    // KNOCKBACK
    // =======================
    private void ApplyKnockback(Collision2D collision)
    {
        PlayerMovement movement = collision.gameObject.GetComponent<PlayerMovement>();
        if (movement == null) return;

        Vector2 direction = (collision.transform.position - transform.position).normalized;
        movement.ApplyKnockback(direction, knockbackForce, knockbackDuration);
    }
}
