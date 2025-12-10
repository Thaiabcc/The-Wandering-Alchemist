using UnityEngine;
using System.Collections; // Cần cái này để dùng Coroutine

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        // Gọi hàm đặt vị trí nhưng chờ 1 xíu
        StartCoroutine(SetPositionWithDelay());
    }

    IEnumerator SetPositionWithDelay()
    {
        // 1. Chờ 1 khung hình (Để đảm bảo mọi thứ khác đã load xong)
        yield return null;

        // 2. Kiểm tra dữ liệu
        if (GameManager.Instance != null && GameManager.Instance.nextSpawnPosition != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                Vector3 targetPos = GameManager.Instance.nextSpawnPosition.Value;

                // 3. DI CHUYỂN CƯỠNG CHẾ
                player.transform.position = targetPos;

                // (Nếu dùng NavMesh hoặc Rigidbody, đôi khi cần reset physics)
                // Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                // if(rb) rb.position = targetPos;

                Debug.Log($"<color=green>Đã dịch chuyển Player đến: {targetPos}</color>");

                // 4. Xóa dữ liệu sau khi dùng
                GameManager.Instance.nextSpawnPosition = null;
            }
            else
            {
                Debug.LogError("Không tìm thấy Player (Kiểm tra Tag!)");
            }
        }
    }
}