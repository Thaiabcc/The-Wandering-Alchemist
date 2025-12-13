using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage; // Kéo cái hình màu Đỏ vào đây
    [SerializeField] private GameObject canvasObject; // Kéo cái HealthBar_Canvas vào đây

    private void Start()
    {
        // Lúc đầu ẩn thanh máu đi cho đỡ rối mắt (Chỉ hiện khi bị đánh)
        // Hoặc để hiện luôn tùy bạn. Dưới đây là logic: Luôn hiện.
        canvasObject.SetActive(true);
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (fillImage != null)
        {
            // Tính phần trăm máu (Ép kiểu float để chia có số lẻ)
            float ratio = (float)currentHealth / maxHealth;
            Debug.Log("Tỷ lệ máu: " + ratio);

            // Cập nhật thanh máu
            fillImage.fillAmount = ratio;
        }

        // Tùy chọn: Nếu chết thì tắt thanh máu luôn
        if (currentHealth <= 0)
        {
            canvasObject.SetActive(false);
        }
    }
}