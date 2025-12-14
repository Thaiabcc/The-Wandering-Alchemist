using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    // Singleton vẫn giữ nguyên
    public static PlayerHealthUI Instance { get; private set; }

    [Header("Máu (Health)")]
    [SerializeField] private Image healthFillImage;

    [Header("Thể lực (Stamina)")]
    [SerializeField] private Image staminaFillImage; // <--- THÊM DÒNG NÀY

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Hàm cập nhật Máu cũ
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthFillImage != null)
        {
            float ratio = (float)currentHealth / maxHealth;
            healthFillImage.fillAmount = ratio;
        }
    }

    // --- HÀM CẬP NHẬT THỂ LỰC MỚI ---
    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        if (staminaFillImage != null)
        {
            float ratio = currentStamina / maxStamina;
            staminaFillImage.fillAmount = ratio;
        }
    }
}