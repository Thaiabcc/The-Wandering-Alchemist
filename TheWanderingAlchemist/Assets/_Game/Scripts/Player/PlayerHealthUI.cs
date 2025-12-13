using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Nhớ dòng này để dùng Image

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance { get; private set; } // Singleton để gọi cho dễ

    [SerializeField] private Image fillImage; // Kéo cái thanh đỏ vào đây

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Hủy cái Canvas mới (trống trơn) đi
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ cái Canvas cũ lại
        }
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (fillImage != null)
        {
            // Ép kiểu float để chia có số lẻ (y hệt cái Slime)
            float ratio = (float)currentHealth / maxHealth;
            fillImage.fillAmount = ratio;
        }
    }
}
