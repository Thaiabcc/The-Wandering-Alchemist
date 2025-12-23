using UnityEngine;
using System.Collections; // Cần cái này cho Coroutine

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Dependencies")]
    // [QUAN TRỌNG] Tham chiếu tới EnemyAI
    // Vì EnemySlime và EnemySkeleton đều là con của EnemyAI, nên kéo vào đây OK hết
    [SerializeField] private EnemyAI enemyAI;

    [Header("Hiệu ứng")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;

    [Header("UI")]
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("Loot")]
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private float dropChance = 50f;
    [SerializeField] private float destroyDelay = 1f;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);

        StartCoroutine(FlashRoutine());

        if (currentHealth <= 0) Die();
        if (currentHealth <= maxHealth / 2)
        {
            // Thử tìm xem có script FlyingRangeBoss không
            var bossAI = GetComponent<FlyingRangeBoss>();
            if (bossAI != null)
            {
                bossAI.ActivateRage();
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {
        if (enemyAI != null) enemyAI.TriggerDeath();

        if (lootPrefab != null && Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }

        // if (QuestManager.Instance != null) QuestManager.Instance.AddKill();

        Destroy(gameObject, destroyDelay);
    }
}