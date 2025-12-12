using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm này sẽ được gọi bởi Vũ khí của người chơi
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} bị đánh! Máu còn: {currentHealth}");

        // Hiệu ứng nhấp nháy đỏ/trắng (Optional - Làm sau)
        // PlayHurtAnimation();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} đã tạch!");

        // Rớt đồ (Loot) - Tính năng này làm sau
        // SpawnLoot();

        Destroy(gameObject);
    }
}