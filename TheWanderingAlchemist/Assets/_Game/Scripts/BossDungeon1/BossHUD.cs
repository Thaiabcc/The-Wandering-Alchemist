using UnityEngine;
using UnityEngine.UI; // Bắt buộc để dùng Slider

public class BossHUD : MonoBehaviour
{
    [Header("Thanh Máu (Màu Đỏ)")]
    public Slider hpSlider;

    [Header("Thanh Poise - Sức Bền (Màu Vàng)")]
    public Slider poiseSlider;

    // Hàm setup ban đầu (Gọi khi gặp Boss)
    public void SetMaxStats(float maxHP, float maxPoise)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP;
        }

        if (poiseSlider != null)
        {
            poiseSlider.maxValue = maxPoise;
            poiseSlider.value = maxPoise;
        }

        // Hiện HUD lên (phòng trường hợp đang ẩn)
        // gameObject.SetActive(true);
    }

    public void UpdateHP(float currentHP)
    {
        if (hpSlider != null) hpSlider.value = currentHP;
    }

    public void UpdatePoise(float currentPoise)
    {
        if (poiseSlider != null) poiseSlider.value = currentPoise;
    }
}