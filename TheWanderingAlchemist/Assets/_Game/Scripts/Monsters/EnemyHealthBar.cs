using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject canvasObject; // Object chứa thanh máu

    [Header("Settings")]
    [Tooltip("Ẩn thanh máu khi quái chết?")]
    [SerializeField] private bool hideOnDeath = true;

    [Tooltip("Ẩn thanh máu khi quái đang đầy máu?")]
    [SerializeField] private bool hideWhenFull = false;

    private void Start()
    {
        // Khởi tạo trạng thái ẩn/hiện dựa trên cài đặt
        if (hideWhenFull)
        {
            SetVisible(false);
        }
        else
        {
            SetVisible(true);
        }
    }

    // Đổi int -> float để khớp với EnemyHealth
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // 1. Phòng trường hợp chia cho 0
        if (maxHealth <= 0) return;

        // 2. Tính toán tỷ lệ (Clamp01 giúp giữ số luôn từ 0 đến 1)
        float fillRatio = Mathf.Clamp01(currentHealth / maxHealth);

        // 3. Cập nhật hình ảnh
        if (fillImage != null)
        {
            fillImage.fillAmount = fillRatio;
        }

        // 4. Xử lý logic Ẩn/Hiện
        HandleVisibility(currentHealth, maxHealth);
    }

    private void HandleVisibility(float current, float max)
    {
        // Nếu chết và bật chế độ ẩn khi chết
        if (current <= 0 && hideOnDeath)
        {
            SetVisible(false);
            return;
        }

        // Nếu đầy máu và bật chế độ ẩn khi đầy
        if (current >= max && hideWhenFull)
        {
            SetVisible(false);
            return;
        }

        // Các trường hợp còn lại thì hiện
        SetVisible(true);
    }

    private void SetVisible(bool isActive)
    {
        if (canvasObject != null && canvasObject.activeSelf != isActive)
        {
            canvasObject.SetActive(isActive);
        }
    }
}