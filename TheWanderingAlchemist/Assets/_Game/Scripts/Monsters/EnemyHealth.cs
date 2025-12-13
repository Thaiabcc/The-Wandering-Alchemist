using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    // --- THÊM DÒNG NÀY ---
    [Header("UI")]
    [SerializeField] private EnemyHealthBar healthBar;
    // ---------------------

    [Header("Phần thưởng (Loot)")]
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private float dropChance = 100f;

    private void Start()
    {
        currentHealth = maxHealth;

        // --- Cập nhật lần đầu cho chắc ---
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // --- GỌI THANH MÁU CẬP NHẬT ---
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        // ------------------------------

        // Debug.Log cũ có thể xóa hoặc comment lại cho đỡ rác
        // Debug.Log(...); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (lootPrefab != null)
        {
            if (Random.Range(0, 100) <= dropChance)
            {
                Instantiate(lootPrefab, transform.position, Quaternion.identity);
            }
        }
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddKill();
        }
        // ---------------------

        Destroy(gameObject);
    }
}
