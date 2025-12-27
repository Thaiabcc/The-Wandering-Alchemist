using UnityEngine;
using UnityEngine.SceneManagement;

public class WagonEntrance : MonoBehaviour, IInteractable
{
    [Header("Cài đặt Map Bên Trong")]
    [SerializeField] private string interiorSceneName = "WagonInterior";
    [SerializeField] private Vector3 spawnPosInside = new Vector3(0, -2, 0);

    [Header("Cài đặt Vị Trí Hồi Hương")]
    [Tooltip("Kéo cái Empty Object (ReturnPoint) bro vừa tạo vào đây")]
    [SerializeField] private Transform returnPoint; // <-- CÁI MỚI

    public void Interact()
    {
        // 1. Lưu tên Map hiện tại
        GameManager.Instance.lastWorldScene = SceneManager.GetActiveScene().name;

        // 2. TÍNH VỊ TRÍ QUAY VỀ (SỬA Ở ĐÂY)
        // Nếu bro quên kéo ReturnPoint thì nó tự tính toán như cũ để chống lỗi
        Vector3 returnPos;

        if (returnPoint != null)
        {
            returnPos = returnPoint.position; // Lấy vị trí chuẩn của cái cọc tiêu
        }
        else
        {
            // Dự phòng nếu lười tạo point
            returnPos = transform.position + new Vector3(0, -2.0f, 0); // Tăng khoảng cách lên -2 cho đỡ kẹt
            Debug.LogWarning("Chưa gắn ReturnPoint! Đang tính toán thủ công.");
        }

        // Đảm bảo Z = 0 để không bị lỗi tàng hình
        returnPos.z = 0;

        // 3. Lưu vào GameManager
        GameManager.Instance.lastWorldPosition = returnPos;

        // 4. Set vị trí spawm trong xe
        GameManager.Instance.nextSpawnPosition = spawnPosInside;

        // 5. Chuyển cảnh (Có Fade đen)
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(interiorSceneName);
        }
        else
        {
            SceneManager.LoadScene(interiorSceneName);
        }
    }
}