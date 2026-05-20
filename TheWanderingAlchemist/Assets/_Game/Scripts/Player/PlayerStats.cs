using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    #region TÀI NGUYÊN & BIẾN (VARIABLES)
    public static PlayerStats Instance { get; private set; }

    [Header("Health & Shield")] 
    [SerializeField] private int maxHealth = 100;
    public float currentHealth;
    public int MaxHealth => maxHealth;
    public float currentShield = 0f;
    public bool isInvincible = false;
    [SerializeField] private GameObject hitShieldPrefab;
    [SerializeField] private float shieldHitVisualDuration = 0.2f;

    private bool isDead;
    private bool isCurrentlyRespawning = false;

    [Header("Dungeon Light")] 
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D playerLight;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float movingRegenRate = 5f;

    [Header("Combat")]
    public float baseDamage = 10f;
    public float currentDamage;

    [Header("Hit Flash")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Poison Debuff")]
    public bool isPoisoned = false;
    public bool isPoisonImmune = false;
    [SerializeField] private Color poisonColor = Color.green; 
    [SerializeField] private Sprite poisonIcon; 
    private Coroutine poisonCoroutine;
    private Coroutine immunityCoroutine;

    private Animator animator;
    private PlayerMovement movement;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool isFlashing = false;

    private Coroutine damageBuffCoroutine;
    private float lastDamageBuffAmount;
    private Coroutine maxHealthBuffCoroutine;
    private int lastMaxHealthBuffAmount;
    private int originalMaxHealth;
    #endregion

    #region UNITY LIFECYCLE
    private void Awake()
    {
        SetupSingleton();
        CacheComponents();
        InitStats();
    }

    private void Start() => UpdateUI();

    private void Update()
    {
        if (isDead) return;
        HandleStaminaRegeneration();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckLightBasedOnScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckLightBasedOnScene;
    }

    private void SetupSingleton()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void CacheComponents()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void InitStats()
    {
        if (originalMaxHealth == 0) originalMaxHealth = maxHealth;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentDamage = baseDamage;
        currentShield = 0f;
        isDead = false;
    }
    #endregion

    #region HEALTH & DEATH
    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;

        float remainingDamage = damage;
        bool shieldBlockedSomething = false;

        if (currentShield > 0)
        {
            shieldBlockedSomething = true;
            if (currentShield >= remainingDamage)
            {
                currentShield -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= currentShield;
                currentShield = 0;
            }

            if (currentShield <= 0 && BuffUIManager.Instance != null)
            {
                BuffUIManager.Instance.RemoveBuff("Shield");
            }
        }

        if (shieldBlockedSomething && hitShieldPrefab != null)
        {
            GameObject activeShield = Instantiate(hitShieldPrefab, transform.position, Quaternion.identity, transform);
            StartCoroutine(DestroyShieldVisual(activeShield, shieldHitVisualDuration));
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
            if (!isFlashing) StartCoroutine(FlashEffect());
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerTakeDamage);
        }

        UpdateUI();
        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    public IEnumerator HealOverTime(float amountPerSecond, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration && !isDead)
        {
            Heal(Mathf.RoundToInt(amountPerSecond));
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    private void Die()
    {
        if (isDead || isCurrentlyRespawning) return;

        isDead = true;
        isCurrentlyRespawning = true;
        
        CurePoison();

        if (movement != null) movement.enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
        if (animator != null) animator.SetTrigger("die");

        if (DeathUI.Instance != null)
        {
            DeathUI.Instance.ResetUI();
            StartCoroutine(DeathUI.Instance.FadeInBlack(1.6f));
        }

        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(2.0f);

        Vector3 spawnPosition = Vector3.zero;
        GameObject spawnObj = GameObject.Find("PlayerSpawnPoint");
        spawnPosition = (spawnObj != null) ? spawnObj.transform.position : transform.position;

        string currentScene = SceneManager.GetActiveScene().name;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.SwitchScene(currentScene);
        else
            SceneManager.LoadScene(currentScene);

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == currentScene);

        yield return new WaitForSeconds(0.8f);

        transform.position = spawnPosition;
        Physics2D.SyncTransforms();

        HealFullAndReset();

        PlayerPenalty penalty = GetComponent<PlayerPenalty>();
        if (penalty != null) penalty.ApplyPenalty();

        if (DeathUI.Instance != null)
        {
            yield return StartCoroutine(DeathUI.Instance.FadeOutBlack(1.5f));
        }

        isCurrentlyRespawning = false;
    }

    public void HealFullAndReset()
    {
        isDead = false;
        maxHealth = originalMaxHealth;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentShield = 0f;
        isInvincible = false;
        
        CurePoison();

        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = Vector2.zero;
        }

        if (BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.RemoveBuff("Shield");
            BuffUIManager.Instance.RemoveBuff("DamageBuff");
            BuffUIManager.Instance.RemoveBuff("MaxHealthBuff");
            BuffUIManager.Instance.RemoveBuff("Poison");
        }

        EnablePlayer(true);
        ResetAnimator();
        ResetSpriteColor();
        UpdateUI();
    }
    #endregion

    #region BUFFS & COMBAT
    public void AddShield(float amount, Sprite buffIcon = null) 
    { 
        currentShield += amount; 
        if (BuffUIManager.Instance != null && buffIcon != null)
        {
            BuffUIManager.Instance.AddBuff("Shield", buffIcon, 0f);
        }
        UpdateUI(); 
    }

    private IEnumerator DestroyShieldVisual(GameObject shieldObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (shieldObject != null)
        {
            Destroy(shieldObject);
        }
    }

    public void ApplyBuffDamage(float amount, float duration, Sprite buffIcon = null)
    {
        if (damageBuffCoroutine != null) StopCoroutine(damageBuffCoroutine);
        else { currentDamage += amount; lastDamageBuffAmount = amount; }

        if (BuffUIManager.Instance != null && buffIcon != null)
        {
            BuffUIManager.Instance.AddBuff("DamageBuff", buffIcon, duration);
        }

        damageBuffCoroutine = StartCoroutine(DamageBuffRoutine(duration));
    }

    private IEnumerator DamageBuffRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentDamage -= lastDamageBuffAmount;
        damageBuffCoroutine = null;
    }

    public void ApplyTemporaryMaxHealth(float amount, float duration, Sprite buffIcon = null)
    {
        if (maxHealthBuffCoroutine != null)
        {
            StopCoroutine(maxHealthBuffCoroutine);
            maxHealth = originalMaxHealth;
        }
        float actualPercent = (amount > 1.0f) ? amount / 100f : amount;
        lastMaxHealthBuffAmount = Mathf.RoundToInt(originalMaxHealth * actualPercent);
        maxHealth += lastMaxHealthBuffAmount;
        currentHealth += lastMaxHealthBuffAmount;

        if (BuffUIManager.Instance != null && buffIcon != null)
        {
            BuffUIManager.Instance.AddBuff("MaxHealthBuff", buffIcon, duration);
        }

        UpdateUI();
        maxHealthBuffCoroutine = StartCoroutine(MaxHealthBuffRoutine(duration));
    }

    private IEnumerator MaxHealthBuffRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        maxHealth = originalMaxHealth;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        maxHealthBuffCoroutine = null;
        UpdateUI();
    }
    #endregion

    #region POISON SYSTEM
    public void ApplyPoison(float damage, float interval)
    {
        if (isDead) return;

        if (isPoisonImmune)
        {
            if (BuffUIManager.Instance != null) 
            {
                BuffUIManager.Instance.RemoveBuff("Poison");
            }
            isPoisoned = true;
            return; 
        }

        if (isPoisoned || poisonCoroutine != null) return;

        isPoisoned = true;
        poisonCoroutine = StartCoroutine(PoisonRoutine(damage, interval));
        
        if (BuffUIManager.Instance != null && poisonIcon != null) 
        {
            BuffUIManager.Instance.AddBuff("Poison", poisonIcon, 0f);
        }
    }

    public void CurePoison()
    {
        isPoisoned = false;
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
            poisonCoroutine = null;
        }

        if (BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.RemoveBuff("Poison");
        }

        ResetSpriteColor();
    }

    public void ApplyPoisonImmunity(float duration, Sprite immunityIcon)
    {
        if (BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.RemoveBuff("Poison");
        }
        
        if (immunityCoroutine != null) StopCoroutine(immunityCoroutine);
        immunityCoroutine = StartCoroutine(PoisonImmunityRoutine(duration, immunityIcon));
    }

    private IEnumerator PoisonImmunityRoutine(float duration, Sprite icon)
    {
        isPoisonImmune = true;
        if (BuffUIManager.Instance != null && icon != null)
        {
            BuffUIManager.Instance.AddBuff("PoisonImmunity", icon, duration);
        }

        yield return new WaitForSeconds(duration);

        isPoisonImmune = false;
        immunityCoroutine = null;

        if (isPoisoned && poisonIcon != null && BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.AddBuff("Poison", poisonIcon, 0f);
        }
    }

    private IEnumerator PoisonRoutine(float damage, float interval)
    {
        while (isPoisoned && !isDead)
        {
            yield return new WaitForSeconds(interval);
            
            if (isPoisonImmune) continue;
            
            if (!isFlashing && sprite != null)
            {
                StartCoroutine(PoisonFlashEffect());
            }
            
            TakeDamage(damage); 
        }
    }

    private IEnumerator PoisonFlashEffect()
    {
        isFlashing = true;
        sprite.color = poisonColor;
        yield return new WaitForSeconds(flashDuration);
        ResetSpriteColor();
        isFlashing = false;
    }
    #endregion

    #region STAMINA SYSTEM
    public bool TryConsumeStamina(float amount)
    {
        if (currentStamina < amount) return false;
        currentStamina -= amount;
        UpdateStaminaUI();
        return true;
    }

    private void HandleStaminaRegeneration()
    {
        if (currentStamina >= maxStamina) return;
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        float actualRegenRate = isMoving ? movingRegenRate : staminaRegenRate;
        currentStamina = Mathf.Min(currentStamina + actualRegenRate * Time.deltaTime, maxStamina);
        UpdateStaminaUI();
    }

    public void RegenerateStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        UpdateStaminaUI();
    }
    #endregion

    #region VISUALS & UTILITIES
    public void BecomeInvincible(float duration)
    {
        StopCoroutine("InvincibilityRoutine");
        StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            SetSpriteAlpha(0f);
            yield return new WaitForSeconds(blinkInterval);
            SetSpriteAlpha(1f);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += (blinkInterval * 2);
        }
        SetSpriteAlpha(1f);
        isInvincible = false;
    }

    private void SetSpriteAlpha(float alpha)
    {
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = alpha;
            sprite.color = c;
        }
    }

    private IEnumerator FlashEffect()
    {
        isFlashing = true;
        sprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        ResetSpriteColor();
        isFlashing = false;
    }

    private void ResetSpriteColor()
    {
        if (sprite == null) return;
        PlayerPenalty penalty = GetComponent<PlayerPenalty>();
        if (penalty != null && penalty.IsInPenalty)
            sprite.color = penalty.debuffColor;
        else
            sprite.color = Color.white;
    }

    private void ResetAnimator()
    {
        if (animator == null) return;
        animator.Rebind();
        animator.Update(0f);
        animator.Play("Idle");
    }

    private void EnablePlayer(bool v)
    {
        if (movement != null) movement.enabled = v;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = v;
    }

    private void CheckLightBasedOnScene(Scene scene, LoadSceneMode mode)
    {
        if (playerLight == null) return;
        if (scene.name == "Dungeon_01")
        {
            playerLight.enabled = true;
        }
        else
        {
            playerLight.enabled = false;
        }
    }
    #endregion

    #region UI UPDATES
    private void UpdateUI()
    {
        if (PlayerHealthUI.Instance == null) return;
        PlayerHealthUI.Instance.UpdateHealth((int)currentHealth, maxHealth);
        PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
    }

    private void UpdateStaminaUI()
    {
        if (PlayerHealthUI.Instance != null)
            PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
    }
    #endregion
}