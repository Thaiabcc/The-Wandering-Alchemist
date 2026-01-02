using UnityEngine;

public class DeflectZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private int counterDamage = 20;

    // 👇 [MỚI] Thời gian bất tử (phải khai báo biến này để truyền sang Player)
    [SerializeField] private float iframeDuration = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();

            if (enemyRb != null)
            {
                // Logic tính hướng đẩy (Giữ nguyên của bạn - Đã chuẩn)
                Transform playerTransform = transform.parent;
                Vector2 originPos = playerTransform != null ? playerTransform.position : transform.position;
                Vector2 targetPos = collision.transform.position;
                Vector2 pushDirection = (targetPos - originPos).normalized;

                enemyRb.velocity = Vector2.zero;
                enemyRb.AddForce(pushDirection * knockbackForce, ForceMode2D.Impulse);

                CameraShake.Instance?.Shake(0.1f, 5f);
                // Audio
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.deflectSuccess, 1.2f);
                HitStop.Instance?.Stop(0.1f);
                Debug.Log("Đã đẩy lùi quái về hướng: " + pushDirection);
            }

            // Gây dame (Giữ nguyên)
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(counterDamage);
                DamagePopupGenerator.Instance?.Create(collision.transform.position, counterDamage, true);
            }

            // 👇 [QUAN TRỌNG] GỌI HÀM BẤT TỬ CHO PLAYER
            // Tìm script PlayerStats ở object cha (Player)
            PlayerStats playerStats = GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.BecomeInvincible(iframeDuration);
            }
        }
    }
}