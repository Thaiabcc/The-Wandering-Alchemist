using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupGenerator : MonoBehaviour
{
    // 1. Tạo biến tĩnh (static) để truy cập từ mọi nơi
    public static DamagePopupGenerator Instance { get; private set; }

    // 2. Chứa tham chiếu đến Prefab của File 1
    [SerializeField] private Transform pfDamagePopup;

    private void Awake()
    {
        // Gán Instance chính là object này
        if (Instance == null)
        {
            Instance = this;
            // --- THÊM DÒNG NÀY ĐỂ BẤT TỬ ---
            DontDestroyOnLoad(gameObject);
            // -------------------------------
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ có 1 Generator tồn tại
        }
    }

    // Cập nhật hàm Create để nhận thêm tham số isCriticalHit (mặc định là false)
    public void Create(Vector3 position, int damageAmount, bool isCriticalHit = false)
    {
        // Kiểm tra null để tránh lỗi nếu quên kéo Prefab
        if (pfDamagePopup == null)
        {
            Debug.LogError("Chưa kéo Prefab vào DamagePopupGenerator kìa bro!");
            return;
        }

        Transform damagePopupTransform = Instantiate(pfDamagePopup, position, Quaternion.identity);

        DamagePopup popup = damagePopupTransform.GetComponent<DamagePopup>();

        // Gọi hàm Setup mới
        if (popup != null)
        {
            popup.Setup(damageAmount, isCriticalHit);
        }
    }
}