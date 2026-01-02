using UnityEngine;

public class EnemySlime : EnemyAI
{
    [Header("Slime Settings")]
    [SerializeField] private int touchDamage = 10;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    // Thêm biến để quái không trừ 1000 máu trong 1 giây
    [SerializeField] private float damageRate = 1.0f;
    private float nextDamageTime = 0f;

    // [QUAN TRỌNG] Đổi từ Enter -> Stay để chạm hướng nào cũng dính
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra Cooldown gây dame
            if (Time.time >= nextDamageTime)
            {
                ApplyDamage(collision);
                ApplyKnockback(collision);
                nextDamageTime = Time.time + damageRate; // Reset hồi chiêu
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
            // Tính hướng từ Quái -> Người (để đẩy người ra)
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            movement.ApplyKnockback(direction, knockbackForce, knockbackDuration);
        }
    }
}