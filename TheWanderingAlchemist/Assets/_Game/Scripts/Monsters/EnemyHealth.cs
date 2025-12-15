using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Dependencies")]
    // Kéo thả script EnemyAI vào đây, hoặc để code tự tìm
    [SerializeField] private EnemyAI enemyAI;

    [Header("UI")]
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("Phần thưởng (Loot)")]
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private float dropChance = 50f;
    [SerializeField] private float destroyDelay = 1f; // Thời gian chờ animation chết chạy xong

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);

        // Tự tìm EnemyAI nếu chưa gán
        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damageAmount)
    {
        // Nếu AI báo đã chết thì không trừ máu nữa (tránh lỗi trừ 2 lần)
        if (enemyAI != null && enemyAI.isDead) return;

        currentHealth -= damageAmount;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 1. Báo cho AI biết để dừng di chuyển và chạy animation
        if (enemyAI != null)
        {
            enemyAI.TriggerDeath();
        }

        // 2. Rớt đồ (Loot)
        if (lootPrefab != null)
        {
            if (Random.Range(0f, 100f) <= dropChance)
            {
                Instantiate(lootPrefab, transform.position, Quaternion.identity);
            }
        }

        // 3. Cập nhật nhiệm vụ
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddKill();
        }

        // 4. QUAN TRỌNG: Không Destroy ngay, mà đợi hết thời gian animation
        Destroy(gameObject, destroyDelay);
    }
}