using UnityEngine;

public class StreetLight : MonoBehaviour
{
    [Header("Trạng thái đèn (Con)")]
    [SerializeField] private GameObject lightOffObject; // Kéo thằng con Light_OFF vào đây
    [SerializeField] private GameObject lightOnObject;  // Kéo thằng con Light_ON vào đây

    private void Start()
    {
        // 1. Đăng ký lắng nghe sự kiện đổi thời gian từ TimeManager
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnNightStateChanged += HandleLightToggle;
            
            // 2. Gọi luôn một lần lúc vừa vào game để đèn sáng/tắt đúng theo giờ hiện tại
            HandleLightToggle(TimeManager.Instance.IsNight);
        }
        else
        {
            Debug.LogWarning("[StreetLight]: Chưa tìm thấy TimeManager Instance trên Scene!");
        }
    }

    private void OnDestroy()
    {
        // Hủy đăng ký khi đối tượng bị xóa (Tránh lỗi Memory Leak của C#)
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnNightStateChanged -= HandleLightToggle;
        }
    }

    // Hàm thực thi việc bật/tắt các object con
    private void HandleLightToggle(bool isNight)
    {
        if (lightOnObject != null && lightOffObject != null)
        {
            lightOnObject.SetActive(isNight);     // Nếu trời tối (true) -> Hiện đèn sáng
            lightOffObject.SetActive(!isNight);   // Nếu trời sáng (false) -> Hiện đèn tắt
        }
    }
}