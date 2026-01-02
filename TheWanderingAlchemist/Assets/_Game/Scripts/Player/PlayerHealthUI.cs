using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private Image healthFill;

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;

    // 👇 [MỚI] Thêm phần cài đặt màu sắc
    [Header("Stamina Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 1f, 0.2f); // Xanh lá (Mặc định)
    [SerializeField] private Color warningColor = Color.yellow;             // Vàng (Cảnh báo)
    [SerializeField] private Color criticalColor = Color.red;               // Đỏ (Nguy hiểm)

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ------------------ HEALTH ------------------

    public void UpdateHealth(int current, int max)
    {
        if (!healthFill) return;

        healthFill.fillAmount = (float)current / max;
    }

    // ------------------ STAMINA ------------------

    public void UpdateStamina(float current, float max)
    {
        if (!staminaFill) return;

        // 1. Tính tỷ lệ phần trăm (0.0 đến 1.0)
        float ratio = current / max;
        staminaFill.fillAmount = ratio;

        // 2. [MỚI] Logic đổi màu theo yêu cầu của bạn
        if (ratio < 0.15f) // Dưới 15% -> Đỏ chót
        {
            staminaFill.color = criticalColor;
        }
        else if (ratio <= 0.5f) // Từ 15% đến 50% -> Vàng cảnh báo
        {
            staminaFill.color = warningColor;
        }
        else // Trên 50% -> Xanh lá
        {
            staminaFill.color = normalColor;
        }
    }
}