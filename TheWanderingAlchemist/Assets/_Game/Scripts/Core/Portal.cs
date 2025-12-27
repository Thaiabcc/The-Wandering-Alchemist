using UnityEngine;
using UnityEngine.SceneManagement;

// Thêm cái IInteractable vào để nó nhận phím E
public class Portal : MonoBehaviour, IInteractable
{
    [Header("Cài đặt Cổng Dungeon")]
    [SerializeField] private string sceneToLoad;      // Tên map muốn đến
    [SerializeField] private Vector3 targetPosition;  // Vị trí xuất hiện ở map kia

    [Header("Cài đặt Hồi Hương")]
    [Tooltip("Kéo cái Empty Object (ReturnPoint) vào đây để khi quay về đứng đúng chỗ")]
    [SerializeField] private Transform returnPoint;

    // Hàm này sẽ chạy khi bro đứng gần và bấm E
    public void Interact()
    {
        Debug.Log("Đang mở cổng...");

        // 1. Lưu lại địa chỉ nhà hiện tại (để sau này dungeon xong còn biết đường về)
        GameManager.Instance.lastWorldScene = SceneManager.GetActiveScene().name;

        // 2. Lưu tọa độ quay về (Sử dụng ReturnPoint cho chuẩn)
        Vector3 myReturnPos = transform.position; // Mặc định là đứng ngay tại cửa
        if (returnPoint != null)
        {
            myReturnPos = returnPoint.position;
        }
        myReturnPos.z = 0; // Chống lỗi tàng hình

        GameManager.Instance.lastWorldPosition = myReturnPos;

        // 3. Gửi tọa độ spawn cho map mới
        GameManager.Instance.nextSpawnPosition = targetPosition;

        // 4. Chuyển cảnh mượt mà
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}