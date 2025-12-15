using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Chỉ số Sinh tồn")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    // Biến này để đánh dấu xem Player này là "Cũ" hay "Mới"
    public bool isOriginal = false;

    [Header("Hiệu ứng khi trúng đòn")] // [MỚI]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 2;

    [Header("Thể lực (Stamina)")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;

    // Các thành phần tham chiếu [MỚI]
    private Animator animator;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        isOriginal = true;
        DontDestroyOnLoad(gameObject);

        if (currentHealth <= 0) currentHealth = maxHealth;

        // Lấy các component [MỚI]
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Log($"Player khởi tạo tại Scene: {SceneManager.GetActiveScene().name} | Máu: {currentHealth}");
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Logic vị trí cũ (Giữ nguyên)
        if (GameManager.Instance != null)
        {
            Vector3 targetPos = GameManager.Instance.nextSpawnPosition.GetValueOrDefault();
            if (targetPos != Vector3.zero)
            {
                transform.position = targetPos;
            }
        }

        // 2. [MỚI - QUAN TRỌNG] LOGIC HỒI SINH (RESET PLAYER)
        // Vì Player không bị hủy, ta phải "tắm rửa sạch sẽ" cho nó thủ công

        Debug.Log("Scene đã load! Đang hồi sinh Player...");

        // A. Hồi máu & Reset trạng thái chết
        currentHealth = maxHealth;
        isDead = false;

        // B. Bật lại Animator (Reset về Idle)
        if (animator != null)
        {
            animator.Rebind(); // Reset toàn bộ animation về trạng thái ban đầu (Entry)
            animator.Update(0f);
        }

        // C. Bật lại Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // D. Bật lại điều khiển di chuyển
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // E. Bật lại khả năng tấn công (nếu có script Attack)
        // PlayerAttack attack = GetComponent<PlayerAttack>();
        // if (attack != null) attack.enabled = true;

        // F. Cập nhật lại thanh máu UI
        if (PlayerHealthUI.Instance != null)
        {
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }
    /*IEnumerator GameOverSequence()
    {
        Debug.Log("Chờ 2 giây để load lại...");

        // Dùng WaitForSecondsRealtime để kể cả khi Time.timeScale = 0 thì nó vẫn đếm
        yield return new WaitForSecondsRealtime(2f);

        Debug.Log("Đang load lại Scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }*/

    private void Start()
    {
        if (isOriginal && currentStamina <= 0) { currentStamina = maxStamina; }

        if (PlayerHealthUI.Instance != null)
        {
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // --- Logic Stamina (Giữ nguyên) ---
    public bool TryConsumeStamina(float amount)
    {
        if (currentStamina > 0)
        {
            currentStamina -= amount;
            if (currentStamina < 0) currentStamina = 0;
            if (PlayerHealthUI.Instance != null)
                PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
            return true;
        }
        return false;
    }
    public void RegenerateStamina(float amount)
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += amount;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            if (PlayerHealthUI.Instance != null)
                PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
        }
    }

    // --- Logic Máu ---
    public void Heal(int amount)
    {
        if (isDead) return;
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log($"<color=green>Đã hồi {amount} máu. HP: {currentHealth}/{maxHealth}</color>");

        if (PlayerHealthUI.Instance != null)
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Á á! Đau quá! Máu còn: {currentHealth}");

        // 1. Cập nhật UI
        if (PlayerHealthUI.Instance != null)
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);

        // 2. [MỚI] Hiệu ứng nháy đỏ
        StartCoroutine(FlashEffect());

        // 3. Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // [MỚI] Coroutine nháy đỏ
    private IEnumerator FlashEffect()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    // [MỚI] Xử lý Chết
    private void Die()
    {
        Debug.Log("GAME OVER - Bạn đã tạch!");
        isDead = true;

        // Reset màu về trắng (tránh bị kẹt màu đỏ)
        StopAllCoroutines();
        spriteRenderer.color = Color.white;

        // Chạy animation Die
        if (animator != null) animator.SetTrigger("die");

        // Dừng di chuyển và tắt điều khiển
        if (playerMovement != null)
        {
            playerMovement.StopMoving(); // Gọi hàm phanh gấp bên kia
            playerMovement.enabled = false; // Tắt luôn script
        }

        // Tắt khả năng tấn công (nếu có script PlayerAttack thì bạn uncomment dòng dưới)
        // PlayerAttack attack = GetComponent<PlayerAttack>();
        // if (attack != null) attack.enabled = false;

        // Tắt va chạm để quái không cắn xác chết
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Gọi Game Over sau 2 giây
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2f);
        // Load lại màn chơi hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        // Cheat code test game
        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(20);
        if (Input.GetKeyDown(KeyCode.H)) Heal(20);
    }
}