using UnityEngine;
using UnityEngine.SceneManagement;

public class WagonExit : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // 1. Lấy địa chỉ cũ ra xem
        string sceneToLoad = GameManager.Instance.lastWorldScene;
        Vector3 posToSpawn = GameManager.Instance.lastWorldPosition;

        // Phòng hờ nếu test trực tiếp trong xe mà không có dữ liệu cũ
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneToLoad = "Town_01"; // Mặc định về Town
            Debug.Log("Không nhớ đường về, về tạm Town!");
        }

        // 2. Dặn dò vị trí spawn (Về lại cạnh xe)
        GameManager.Instance.nextSpawnPosition = posToSpawn;

        // 3. Về nhà thôi
        SceneManager.LoadScene(sceneToLoad);
    }
}