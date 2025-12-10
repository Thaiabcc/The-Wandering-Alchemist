using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton: Để gọi GameManager.Instance từ bất cứ đâu
    public static GameManager Instance { get; private set; }

    // Biến lưu vị trí spawn tiếp theo (Dùng cho Portal)
    // Dấu ? nghĩa là nó có thể Null (chưa có vị trí nào)
    public Vector3? nextSpawnPosition = null;
    public string lastWorldScene;
    public Vector3 lastWorldPosition;

    private void Awake()
    {
        // Đảm bảo chỉ có 1 GameManager duy nhất tồn tại
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Nếu có cái thứ 2 sinh ra -> Hủy ngay
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // QUAN TRỌNG: Giữ nó không bị xóa khi Load Scene
        }
    }
    private void OnDestroy()
    {
        Debug.LogError("GAME MANAGER VỪA BỊ HỦY! (Mất dữ liệu)");
    }
}