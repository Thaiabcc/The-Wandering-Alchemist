using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    #region Configuration
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float dropChance = 100f;
    [SerializeField] private float destroyDelay = 1f;

    [Header("Quest Info")]
    [SerializeField] private string enemyNameForQuest = "Slime";

    [Header("Visual Effects")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("References")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private EnemyHealthBar healthBar;
    
    private EnemyAudio enemyAudio;
    #endregion

    #region State Variables
    private float currentHealth;
    private FlyingRangeBoss bossScript;
    private WaitForSeconds flashWait;
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
    public void TakeDamage(float damageAmount, bool isCritical = false, bool isPoison = false)
    {
        if (isDead || currentHealth <= 0) return;

        ApplyDamage(damageAmount);
        ProcessBossLogic(damageAmount);
        UpdateUI();
        PlayHitEffect();
        enemyAudio?.PlayHit();

        if (DamagePopupGenerator.Instance != null)
        {
            DamagePopupGenerator.Instance.Create(transform.position, (int)damageAmount, isCritical, isPoison);
        }

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
        isDead = false;
    }

    private void InitializeComponents()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();
        bossScript = GetComponent<FlyingRangeBoss>();
        
        enemyAudio = GetComponent<EnemyAudio>();
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

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (bossScript != null && bossScript.bossHUD != null)
            bossScript.bossHUD.gameObject.SetActive(false);

        if (enemyAI != null)
            enemyAI.TriggerDeath();

        try
        {
            enemyAudio?.PlayDie();
            HandleLootDrop();
        }
        catch (System.Exception) { }

        try
        {
            HandleQuestProgress();
        }
        catch (System.Exception) { }

        if (bossScript != null)
        {
            BossPersistence bossSave = GetComponent<BossPersistence>();

            if (bossSave != null)
            {
                bossSave.MarkAsDefeated();

                if (SaveManager.Instance != null)
                    SaveManager.Instance.SaveGame();
            }

            StartCoroutine(TriggerEndingScene());
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private IEnumerator TriggerEndingScene()
    {
        yield return new WaitForSeconds(bossScript.delayBeforeEnding);

        if (UIController.Instance != null)
        {
            UIController.Instance.ShowManager(false);
        }

        if (!string.IsNullOrEmpty(bossScript.endingSceneName))
        {
            SceneManager.LoadScene(bossScript.endingSceneName);
        }
    }

    private void HandleQuestProgress()
    {
        if (QuestManager.Instance != null && bossScript == null)
        {
            QuestManager.Instance.AddKill(enemyNameForQuest);
        }
    }

    private void HandleLootDrop()
    {
        if (lootPrefab != null && Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }
    }
}