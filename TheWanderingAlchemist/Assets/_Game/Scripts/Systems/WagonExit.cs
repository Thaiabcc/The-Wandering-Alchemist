using UnityEngine;
using UnityEngine.SceneManagement;

public class WagonExit : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // 1. Lấy địa chỉ cũ ra xem
        string sceneToLoad = GameManager.Instance.lastWorldScene;
        Vector3 posToSpawn = GameManager.Instance.lastWorldPosition;

        // Phòng hờ nếu test trực tiếp trong xe mà không có dữ liệu cũ (Play từ scene Wagon)
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneToLoad = "Town_01"; // Mặc định về Town

            // [QUAN TRỌNG] Nếu không có dữ liệu cũ, đừng để posToSpawn là (0,0,0)
            // Vì (0,0,0) ở Town có thể là giữa hồ nước hoặc trong tường.
            // Nên set tạm một tọa độ an toàn nào đó.
            posToSpawn = new Vector3(5, 5, 0);

            Debug.Log("Không nhớ đường về, về tạm Town tại vị trí mặc định!");
        }

        // 2. Dặn dò vị trí spawn (Về lại cạnh xe hoặc vị trí đã lưu)
        GameManager.Instance.nextSpawnPosition = posToSpawn;

        // 3. Về nhà thôi (Dùng cái này mới có màn hình đen mượt mà)
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(sceneToLoad);
        }
        else
        {
            // Dự phòng
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}