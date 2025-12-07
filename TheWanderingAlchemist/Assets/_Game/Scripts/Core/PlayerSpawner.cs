using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        // Kiểm tra xem có lệnh spawn nào từ map trước không
        if (GameManager.Instance != null && GameManager.Instance.nextSpawnPosition != null)
        {
            // Lấy Player (Tìm theo Tag cho chắc)
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Di chuyển Player đến vị trí đã lưu
                player.transform.position = GameManager.Instance.nextSpawnPosition.Value;

                // Reset lại để lần sau không bị nhảy lung tung
                GameManager.Instance.nextSpawnPosition = null;
            }
        }
    }
}