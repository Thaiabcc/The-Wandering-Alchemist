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

    private bool isDead;

    // ==============================
    // Stamina
    // ==============================
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Tooltip("Tốc độ hồi khi đứng im")]
    public float staminaRegenRate = 10f;

    [Tooltip("Tốc độ hồi khi đang di chuyển (Nên thấp hơn đứng im)")]
    public float movingRegenRate = 5f; // [MỚI] Biến này để chỉnh tốc độ hồi khi đi

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

    // [MỚI] Thêm hàm Update để hồi Stamina theo thời gian thực
    private void Update()
    {
        if (isDead) return;
        HandleStaminaRegeneration();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        if (currentHealth <= 0)
            currentHealth = maxHealth;

        if (currentStamina <= 0)
            currentStamina = maxStamina;
        currentDamage = baseDamage;
    }

    // ==============================
    // Scene Reload / Respawn
    // ==============================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Respawn();
    }

    private void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;

        ResetAnimator();
        EnablePlayer(true);
        UpdateUI();
    }

    private void ResetAnimator()
    {
        if (animator == null) return;

        animator.Rebind();
        animator.Update(0f);
    }

    private void EnablePlayer(bool value)
    {
        if (movement != null)
            movement.enabled = value;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = value;
    }

    // ==============================
    // Logic Hồi Stamina Tự Động [MỚI]
    // ==============================
    private void HandleStaminaRegeneration()
    {
        // Nếu đã đầy cây thì không hồi nữa
        if (currentStamina >= maxStamina) return;

        // Kiểm tra xem người chơi có đang bấm nút di chuyển không
        // (Cách này nhanh nhất, không cần phụ thuộc vào script movement)
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        // Chọn tốc độ hồi dựa trên trạng thái
        float actualRegenRate = isMoving ? movingRegenRate : staminaRegenRate;

        // Cộng Stamina theo thời gian thực
        currentStamina += actualRegenRate * Time.deltaTime;

        // Đảm bảo không vượt quá Max
        if (currentStamina > maxStamina) currentStamina = maxStamina;

        // Cập nhật UI (Nên tối ưu: Chỉ update khi giá trị thay đổi đáng kể để đỡ tốn)
        UpdateStaminaUI();
    }

    // ==============================
    // Health Logic
    // ==============================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateUI();
        StartCoroutine(FlashEffect());

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
    // BuffDame Logic
    // ==============================
    public void ApplyBuffDamage(float amount, float duration)
    {
        StartCoroutine(DamageBuffRoutine(amount, duration));
    }

    private IEnumerator DamageBuffRoutine(float amount, float duration)
    {
        currentDamage += amount;
        Debug.Log($"<color=green>BUFF ACTIVATED:</color> +{amount} Damage. Current: {currentDamage}");

        yield return new WaitForSeconds(duration);

        currentDamage -= amount;
        Debug.Log($"<color=yellow>BUFF ENDED:</color> Damage returned to: {currentDamage}");
    }

    // ==============================
    // Death
    // ==============================
    private void Die()
    {
        isDead = true;

        StopAllCoroutines();
        ResetSpriteColor();

        if (animator != null)
            animator.SetTrigger("die");

        if (movement != null)
        {
            movement.StopMoving();
            movement.enabled = false;
        }

        EnablePlayer(false);
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ==============================
    // Stamina (Manual Consume/Regen)
    // ==============================
    public bool TryConsumeStamina(float amount)
    {
        if (currentStamina < amount) return false; // [FIX] Sửa điều kiện: phải đủ stamina mới cho dùng

        currentStamina -= amount; // Không cần Mathf.Max ở đây vì đã check if ở trên
        UpdateStaminaUI();
        return true;
    }

    // Hàm này dùng cho Potion (hồi tức thì một lượng lớn)
    public void RegenerateStamina(float amount)
    {
        if (currentStamina >= maxStamina) return;

        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        UpdateStaminaUI();
    }

    // ==============================
    // Effects
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
        if (sprite != null)
            sprite.color = Color.white;
    }

    // ==============================
    // UI
    // ==============================
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