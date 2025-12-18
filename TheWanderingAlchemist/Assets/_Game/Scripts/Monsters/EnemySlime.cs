using UnityEngine;

// Kế thừa từ EnemyAI (thay vì EnemyCore)
public class EnemySlime : EnemyAI
{
    [Header("Slime Settings")]
    [SerializeField] private int touchDamage = 10;
    [SerializeField] private float knockbackForce = 5f;

    // Slime dùng va chạm để gây damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
            if (stats != null) stats.TakeDamage(touchDamage);

            PlayerMovement move = collision.gameObject.GetComponent<PlayerMovement>();
            if (move != null)
            {
                Vector2 dir = (collision.transform.position - transform.position).normalized;
                move.ApplyKnockback(dir, knockbackForce, 0.2f);
            }
        }
    }
}