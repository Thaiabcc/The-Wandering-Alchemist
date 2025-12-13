using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Chỉ số Sinh tồn")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    private void Awake()
    {
        // Kiểm tra xem đã có PlayerStats nào tồn tại chưa
        if (Instance != null && Instance != this)
        {
            // Nếu có rồi (từ màn trước mang sang), thì hủy cái Player mới sinh ra này đi
            // Để giữ lại cái cũ (đang chứa số máu hiện tại)
            Destroy(gameObject);
        }
        else
        {
            // Nếu chưa có, thì mình là trùm
            Instance = this;
            DontDestroyOnLoad(gameObject); // <--- CÂU THẦN CHÚ BẤT TỬ
        }

        // Dòng này chỉ chạy nếu đây là lần đầu tiên sinh ra (ở Town)
        // Nếu là Player mang từ màn trước sang thì nó không reset máu nữa
        if (Instance == this)
        {
            currentHealth = maxHealth;
        }
    }

    private void Start()
    {
        // 3. Cập nhật UI ngay khi vào game để thanh máu đầy
        if (PlayerHealthUI.Instance != null)
        {
            PlayerHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
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