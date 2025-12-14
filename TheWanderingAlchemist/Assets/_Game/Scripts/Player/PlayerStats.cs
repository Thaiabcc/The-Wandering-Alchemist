using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Chỉ số Sinh tồn")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    // Biến này để đánh dấu xem Player này là "Cũ" hay "Mới"
    public bool isOriginal = false;

    public string nextSpawnID;
    
    // Thể lực
    [Header("Thể lực (Stamina)")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Có thằng khác tồn tại rồi -> Mình là đồ fake -> Tự hủy
            Destroy(gameObject);
            return; // Quan trọng: Dừng code ngay, không chạy đoạn dưới nữa!
        }

        // Nếu chưa có ai -> Mình là trùm
        Instance = this;
        isOriginal = true; // Đánh dấu tôi là bản gốc
        DontDestroyOnLoad(gameObject);

        // Chỉ set đầy máu nếu đây là lần khởi tạo đầu tiên
        // (Lúc game mới bật, currentHealth thường bằng 0 hoặc giá trị Inspector)
        // Ta giả định nếu currentHealth <= 0 nghĩa là chưa setup bao giờ
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }

        Debug.Log($"Player khởi tạo tại Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} | Máu: {currentHealth}");
    }
    // --- THÊM MỚI ĐOẠN NÀY ---
    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện chuyển cảnh
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Hủy đăng ký khi object bị hủy (để tránh lỗi)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Hàm này sẽ TỰ ĐỘNG CHẠY mỗi khi load xong màn mới
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra xem GameManager có dặn dò vị trí nào không
        if (GameManager.Instance != null)
        {
            // Lấy vị trí cần đến
            Vector3 targetPos = GameManager.Instance.nextSpawnPosition.GetValueOrDefault();
            // Kiểm tra: Nếu vị trí là (0,0,0) thì có thể là chưa set, 
            // nhưng nếu WagonExit set đúng thì nó sẽ khác (0,0,0).
            // Tuy nhiên, để chắc chắn, ta cứ dịch chuyển.

            // QUAN TRỌNG: Cần check xem có phải Vector3.zero mặc định không 
            // (nếu bạn muốn spawn mặc định thì bỏ qua check này)
            if (targetPos != Vector3.zero)
            {
                transform.position = targetPos;

                // Reset lại để lần sau không bị nhảy linh tinh nếu đi bộ bình thường
                // (Tùy logic game, có thể không cần dòng dưới nếu Portal luôn set đè)
                // GameManager.Instance.nextSpawnPosition = Vector3.zero; 
            }
        }
    }

    private void Start()
    {
        // Khởi tạo Stamina
        if(isOriginal && currentStamina <= 0) { currentStamina = maxStamina; }
        // Cập nhật UI
        if (PlayerHealthUI.Instance != null)
        {
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // Logic Stamina
    public bool TryConsumeStamina(float amount)
    {
        if(currentStamina > 0)
        {
            currentStamina -= amount;
            if (currentStamina < 0) currentStamina = 0;

            // Cập nhật UI
            if (PlayerHealthUI.Instance != null)
                PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
            return true;
        }
        return false;
    }
    public void RegenerateStamina(float amount)
    {
        if(currentStamina < maxStamina)
        {
            currentStamina += amount;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            if (PlayerHealthUI.Instance != null)
                PlayerHealthUI.Instance.UpdateStamina(currentStamina, maxStamina);
        }
    }

    // Hàm hồi máu (Dùng khi uống thuốc)
    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Máu đã đầy, không cần uống!");
            return;
        }

        currentHealth += amount;

        // Không cho máu vượt quá Max
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log($"<color=green>Đã hồi {amount} máu. HP: {currentHealth}/{maxHealth}</color>");

        // --- SỬA LỖI: THÊM ĐOẠN NÀY ĐỂ UI CẬP NHẬT KHI HỒI MÁU ---
        if (PlayerHealthUI.Instance != null)
        {
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
        // ---------------------------------------------------------
    }

    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Á á! Đau quá! Máu còn: {currentHealth}");

        // --- GỌI UI CẬP NHẬT ---
        if (PlayerHealthUI.Instance != null)
        {
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
        // ------------------------

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("GAME OVER - Bạn đã tạch!");
    }

    // Phím tắt Debug
    private void Update()
    {
        // Bấm K để tự đánh mình (Test mất máu)
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20);
        }

        // Bấm H để tự hồi máu (Test uống thuốc)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(20);
        }
    }
}