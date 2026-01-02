using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    #region Configuration
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float dropChance = 100f;
    [SerializeField] private float destroyDelay = 1f;

    [Header("Quest Info")]
    [Tooltip("Điền tên quái giống hệt trong Quest của NPC (VD: Slime)")]
    [SerializeField] private string enemyNameForQuest = "Slime";

    [Header("Visual Effects")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("References")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private EnemyHealthBar healthBar;
    #endregion

    #region State Variables
    private float currentHealth;
    private FlyingRangeBoss bossScript;
    private WaitForSeconds flashWait;
    // Biến để đảm bảo chỉ chết 1 lần
    private bool isDead = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeStats();
        InitializeComponents();
        SetupBossConfiguration();
        UpdateUI();
    }
    #endregion

    #region Main Logic
    public void TakeDamage(float damageAmount)
    {
        if (isDead || currentHealth <= 0) return;

        ApplyDamage(damageAmount);
        ProcessBossLogic(damageAmount);
        UpdateUI();
        PlayHitEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    #endregion

    #region Helper Methods (Standard)
    private void InitializeStats()
    {
        currentHealth = maxHealth;
        flashWait = new WaitForSeconds(flashDuration);
        isDead = false;
    }

    private void InitializeComponents()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();
        bossScript = GetComponent<FlyingRangeBoss>();
    }

    private void SetupBossConfiguration()
    {
        if (bossScript != null && bossScript.bossHUD != null)
        {
            if (healthBar != null) healthBar.gameObject.SetActive(false);
            bossScript.bossHUD.SetMaxStats(maxHealth, bossScript.maxPoise);
        }
    }

    private void ApplyDamage(float amount)
    {
        currentHealth -= amount;
    }

    private void UpdateUI()
    {
        if (healthBar != null && healthBar.gameObject.activeSelf)
        {
            healthBar.UpdateHealthBar((int)currentHealth, (int)maxHealth);
        }
        if (bossScript != null && bossScript.bossHUD != null)
        {
            bossScript.bossHUD.UpdateHP(currentHealth);
        }
    }

    private void ProcessBossLogic(float damageAmount)
    {
        if (bossScript == null) return;
        bossScript.TakeDamage(damageAmount);
        if (currentHealth <= maxHealth / 2)
        {
            bossScript.ActivateRage();
        }
    }

    private void PlayHitEffect()
    {
        if (gameObject.activeInHierarchy)
        {
            StopCoroutine(nameof(FlashRoutine));
            StartCoroutine(nameof(FlashRoutine));
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return flashWait;
            spriteRenderer.color = originalColor;
        }
    }
    #endregion

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Tắt va chạm/hình ảnh
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        if (healthBar != null) healthBar.gameObject.SetActive(false);
        if (bossScript != null && bossScript.bossHUD != null) bossScript.bossHUD.gameObject.SetActive(false);
        if (enemyAI != null) enemyAI.TriggerDeath();

        // ====================================================
        // 👇 SỬA ĐOẠN NÀY: TÁCH RIÊNG 2 KHỐI TRY-CATCH RA
        // ====================================================

        // ƯU TIÊN 1: Rơi đồ trước (Dù Quest lỗi thì vẫn phải có tiền!)
        try
        {
            // Audio
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.enemyDie, 0.8f, true);
            HandleLootDrop();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi rơi đồ: " + e.Message);
        }

        // ƯU TIÊN 2: Tính Quest sau
        try
        {
            HandleQuestProgress();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi tính Quest: " + e.Message);
        }

        // ====================================================

        // 4. Hủy Object
        Destroy(gameObject, destroyDelay);
    }

    private void HandleQuestProgress()
    {
        // Kiểm tra kỹ QuestManager có tồn tại không
        if (QuestManager.Instance != null)
        {
            // Chỉ gọi nếu là quái thường (Boss thường có logic riêng, hoặc xóa check bossScript nếu muốn boss cũng tính)
            if (bossScript == null)
            {
                QuestManager.Instance.AddKill(enemyNameForQuest);
            }
        }
    }

    private void HandleLootDrop()
    {
        // 1. Ưu tiên dùng LootManager nếu bro có dùng (mở comment bên dưới nếu dùng)
        /*
        if (LootManager.Instance != null) {
             LootManager.Instance.SpawnLoot(transform.position);
             return;
        }
        */

        // 2. Nếu không dùng Manager thì dùng Prefab có sẵn trong script này
        if (lootPrefab != null && Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }
    }
}