using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số")]
    [SerializeField] private int maxHealth = 100; // Để máu nhiều chút test cho dễ
    private int currentHealth;

    [Header("UI")]
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("Phần thưởng (Loot)")]
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private float dropChance = 50f;

    // [Header("Hiệu ứng")] ---> XÓA DÒNG NÀY
    // [SerializeField] private GameObject damagePopupPrefab; ---> XÓA DÒNG NÀY

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Cập nhật thanh máu
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        // --- ĐOẠN NÀY ĐÃ BỊ XÓA ---
        // Lý do: PlayerAttack đã gọi DamagePopupGenerator rồi.
        // Không cần hiện damage ở đây nữa để tránh bị 2 số đè lên nhau.
        // ---------------------------

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Rớt đồ (Loot)
        if (lootPrefab != null)
        {
            if (Random.Range(0f, 100f) <= dropChance)
            {
                Instantiate(lootPrefab, transform.position, Quaternion.identity);
            }
        }

        // Cập nhật nhiệm vụ
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddKill();
        }

        Destroy(gameObject);
    }
}