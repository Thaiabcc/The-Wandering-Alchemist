using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    private void Awake()
    {
        // Tìm xem trong game đã có thằng nào tên giống mình chưa?
        // (Hoặc tìm theo tag/type tùy logic, nhưng tìm theo tên object là dễ nhất lúc này)

        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        // Lưu ý: Để dùng cách này, bạn nên set Tag cho EventSystem là "GameController"
        // HOẶC dùng cách dưới đây (An toàn hơn cho EventSystem):

        var systems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();

        if (systems.Length > 1)
        {
            // Nếu tìm thấy hơn 1 cái -> Nghĩa là đã có cái cũ sống dai rồi
            // -> Hủy cái mới này đi (Chính là cái của Scene Town mới load)
            Destroy(gameObject);
        }
        else
        {
            // Nếu mình là duy nhất -> Bất tử
            DontDestroyOnLoad(gameObject);
        }
    }
}