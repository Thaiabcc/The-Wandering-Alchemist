using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    // Biến lưu máu toàn cục
    public int currentHealth;
    public int maxHealth = 100;

    // Biến kiểm tra xem đây có phải lần đầu chơi không
    public bool isFirstLoad = true;

    private void Awake()
    {
        // Singleton pattern: Đảm bảo chỉ có 1 Manager duy nhất
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // QUAN TRỌNG: Không hủy khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm gọi khi muốn set lại máu (ví dụ khi Respawn hoặc New Game)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}