using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    // ==============================
    // Singleton
    // ==============================
    public static PlayerStats Instance { get; private set; }

    // ==============================
    // Health
    // ==============================
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;
    public int MaxHealth => maxHealth;

    // Biến kiểm tra bất tử
    public bool isInvincible = false;

    private bool isDead;

    // ==============================
    // Stamina
    // ==============================
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float movingRegenRate = 5f;

    // ==============================
    // Combat
    // ==============================
    [Header("Combat")]
    public float baseDamage = 10f;
    public float currentDamage;

    // ==============================
    // Hit Flash Effect
    // ==============================
    [Header("Hit Flash")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 2;

    // ==============================
    // Components
    // ==============================
    private Animator animator;
    private PlayerMovement movement;
    private SpriteRenderer sprite;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Awake()
    {
        SetupSingleton();
        CacheComponents();
        InitStats();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (isDead) return;
        HandleStaminaRegeneration();
    }

    // ==============================
    // Initialization
    // ==============================
    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void CacheComponents()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void InitStats()
    {
        if (currentHealth <= 0) currentHealth = maxHealth;
        if (currentStamina <= 0) currentStamina = maxStamina;
        currentDamage = baseDamage;
    }

    // ==============================
    // Logic return
    // ==============================
    public void HealFullAndReset()
    {
        isDead = false;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        isInvincible = false;

        // Reset Components
        EnablePlayer(true);
        ResetAnimator();
        ResetSpriteColor();
        UpdateUI();
        
        Debug.Log("Player stats reset success!");
    }

    private void ResetAnimator()
    {
        if (animator == null) return;
        animator.Rebind();
        animator.Update(0f);
        animator.Play("Idle"); 
    }

    private void EnablePlayer(bool value)
    {
        if (movement != null)
        {
            movement.enabled = value;
            if (value) movement.enabled = true; 
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = value;
    }

    // ==============================
    // Stamina Logic
    // ==============================
    private void HandleStaminaRegeneration()
    {
        if (currentStamina >= maxStamina) return;

        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        float actualRegenRate = isMoving ? movingRegenRate : staminaRegenRate;

        currentStamina += actualRegenRate * Time.deltaTime;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    public bool TryConsumeStamina(float amount)
    {
        if (currentStamina < amount) return false;
        currentStamina -= amount;
        UpdateStaminaUI();
        return true;
    }

    public void RegenerateStamina(float amount)
    {
        if (currentStamina >= maxStamina) return;
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        UpdateStaminaUI();
    }

    // ==============================
    // Health Logic
    // ==============================
    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        // Audio
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerTakeDamage);
        UpdateUI();
        StartCoroutine(FlashEffect());

        // CameraShake.Instance?.Shake(0.2f, 3f); // Uncomment nếu có
        // HitStop.Instance?.Stop(0.1f);          // Uncomment nếu có
        
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        if (currentHealth >= maxHealth) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    // ==============================
    // Invincibility Logic
    // ==============================
    public void BecomeInvincible(float duration)
    {
        StopCoroutine("InvincibilityRoutine");
        StartCoroutine(InvincibilityRoutine(duration));
    }
     
    [SerializeField] private float blinkInterval = 0.1f;

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

    // ==============================
    // BuffDame Logic
    // ==============================
    public void ApplyBuffDamage(float amount, float duration)
    {
        StartCoroutine(DamageBuffRoutine(amount, duration));
    }

    private IEnumerator DamageBuffRoutine(float amount, float duration)
    {
        currentDamage += amount;
        yield return new WaitForSeconds(duration);
        currentDamage -= amount;
    }

    // ==============================
    // Death
    // ==============================
    private void Die()
    {
        if (isDead) return; 
        
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerDie);
        isDead = true;
        StopAllCoroutines();
        ResetSpriteColor();

        if (animator != null) animator.SetTrigger("die");

        if (movement != null)
        {
            movement.enabled = false;
        }

        EnablePlayer(false);
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(1.5f);
        if (DeathUI.Instance != null)
        {
            yield return StartCoroutine(DeathUI.Instance.FadeInBlack(1f));
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnPlayer();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield break; 
        }

        yield return new WaitForSeconds(1f);

        if (DeathUI.Instance != null)
        {
            yield return StartCoroutine(DeathUI.Instance.FadeOutBlack(1f));
        }

        EnablePlayer(true);
    }

    // ==============================
    // Effects & UI
    // ==============================
    private IEnumerator FlashEffect()
    {
        if (sprite == null) yield break;
        for (int i = 0; i < flashCount; i++)
        {
            sprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            ResetSpriteColor();
            yield return new WaitForSeconds(flashDuration);
        }
    }

    private void ResetSpriteColor()
    {
        if (sprite != null) sprite.color = Color.white;
    }

    private void UpdateUI()
    {
        if (PlayerHealthUI.Instance == null) return;
        PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
    }

    private void UpdateStaminaUI()
    {
        if (PlayerHealthUI.Instance != null)
            PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
    }
}