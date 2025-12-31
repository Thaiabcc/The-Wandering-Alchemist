using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    #region Configuration
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float dropChance = 50f;
    [SerializeField] private float destroyDelay = 1f;

    [Header("Visual Effects")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("References")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private EnemyHealthBar healthBar; // UI cho quái thường
    #endregion

    #region State Variables
    private float currentHealth;
    private FlyingRangeBoss bossScript; // Reference Boss (nếu có)
    private WaitForSeconds flashWait;   // Cache để tối ưu hiệu năng
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeStats();
        InitializeComponents();
        SetupBossConfiguration();
        UpdateUI(); // Setup UI ban đầu
    }
    #endregion

    #region Main Logic
    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;

        ApplyDamage(damageAmount);
        ProcessBossLogic(damageAmount); // Logic riêng nếu là Boss (Poise, Rage)
        UpdateUI();
        PlayHitEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    #endregion

    #region Helper Methods

    private void InitializeStats()
    {
        currentHealth = maxHealth;
        flashWait = new WaitForSeconds(flashDuration);
    }

    private void InitializeComponents()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();

        // Cố gắng lấy script Boss
        bossScript = GetComponent<FlyingRangeBoss>();
    }

    private void SetupBossConfiguration()
    {
        // Nếu đây là Boss
        if (bossScript != null && bossScript.bossHUD != null)
        {
            // 1. Tắt thanh máu nhỏ (Quái thường)
            if (healthBar != null) healthBar.gameObject.SetActive(false);

            // 2. Setup thanh máu to (BossHUD)
            bossScript.bossHUD.SetMaxStats(maxHealth, bossScript.maxPoise);
        }
    }

    private void ApplyDamage(float amount)
    {
        currentHealth -= amount;
    }

    private void UpdateUI()
    {
        // UI Quái thường
        if (healthBar != null && healthBar.gameObject.activeSelf)
        {
            healthBar.UpdateHealthBar((int)currentHealth, (int)maxHealth);
        }

        // UI Boss (Chỉ update HP, phần Rage/Poise xử lý ở logic Boss)
        if (bossScript != null && bossScript.bossHUD != null)
        {
            bossScript.bossHUD.UpdateHP(currentHealth);
        }
    }

    private void ProcessBossLogic(float damageAmount)
    {
        if (bossScript == null) return;

        // Trừ Poise bên script Boss
        bossScript.TakeDamage(damageAmount);

        // Kích hoạt Rage nếu máu < 50%
        if (currentHealth <= maxHealth / 2)
        {
            bossScript.ActivateRage();
        }
    }

    private void PlayHitEffect()
    {
        if (gameObject.activeInHierarchy) // Chỉ chạy coroutine khi object đang active
        {
            StopCoroutine(nameof(FlashRoutine)); // Reset nếu đang nháy dở
            StartCoroutine(nameof(FlashRoutine));
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;

            yield return flashWait; // Sử dụng biến đã cache

            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        HandleQuestProgress();
        HandleLootDrop();
        if (bossScript != null && bossScript.bossHUD != null) 
        {
            bossScript.bossHUD.gameObject.SetActive(false);
        }

        // Báo cho AI biết đã chết để dừng di chuyển/anim
        if (enemyAI != null) enemyAI.TriggerDeath();

        Destroy(gameObject, destroyDelay);
    }

    private void HandleQuestProgress()
    {
        // Chỉ tính kill cho quái thường (Boss thường có logic Quest riêng hoặc xử lý sau)
        if (QuestManager.Instance != null && bossScript == null)
        {
            QuestManager.Instance.AddKill();
        }
    }

    private void HandleLootDrop()
    {
        if (lootPrefab != null && Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }
    }
    #endregion
}