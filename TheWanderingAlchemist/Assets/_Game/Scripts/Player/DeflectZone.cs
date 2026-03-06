using UnityEngine;

public class DeflectZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private int counterDamage = 20;
    [SerializeField] private float iframeDuration = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();

            if (enemyRb != null)
            {
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

            // Dame
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(counterDamage);
                DamagePopupGenerator.Instance?.Create(collision.transform.position, counterDamage, true);
            }

            PlayerStats playerStats = GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.BecomeInvincible(iframeDuration);
            }
        }
    }
}