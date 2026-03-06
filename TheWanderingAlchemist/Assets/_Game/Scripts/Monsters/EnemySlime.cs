using UnityEngine;

public class EnemySlime : EnemyAI
{
    [Header("Slime Settings")]
    [SerializeField] private int touchDamage = 10;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float damageRate = 1.0f;
    private float nextDamageTime = 0f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                ApplyDamage(collision);
                ApplyKnockback(collision);
                nextDamageTime = Time.time + damageRate;
            }
        }
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
        if (movement != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            movement.ApplyKnockback(direction, knockbackForce, knockbackDuration);
        }
    }
}