using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Chỉ số Sinh tồn")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    private void Awake()
    {
        Instance = this;
        currentHealth = maxHealth; // Mới vào game đầy máu
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
    }

    // Hàm nhận sát thương (Dùng để test)
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        Debug.Log($"<color=red>Bị thương! HP: {currentHealth}/{maxHealth}</color>");
    }

    // Phím tắt Debug để tự làm đau mình (Test thuốc)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20);
        }
    }
}