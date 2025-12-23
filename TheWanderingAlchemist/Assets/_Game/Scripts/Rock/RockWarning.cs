using UnityEngine;

public class RockWarning : MonoBehaviour
{
    [Header("--- CẤU HÌNH ---")]
    public GameObject realRockPrefab; // Kéo Prefab cục đá thật vào đây
    public float warningDuration = 0.8f; // Thời gian hiện cảnh báo trước khi đá rơi
    public float spawnHeight = 10f; // Đá sẽ rơi từ độ cao 10m so với mặt đất

    private float timer;

    void Start()
    {
        timer = warningDuration;
        // Làm hiệu ứng nhấp nháy hoặc to dần (tùy chọn)
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Hiệu ứng vòng tròn to dần ra cho kịch tính
        float scale = 1f - (timer / warningDuration);
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 0.5f, 1f), scale); // Dẹt dẹt cho giống bóng

        if (timer <= 0)
        {
            SpawnRock();
            Destroy(gameObject); // Hủy vòng tròn cảnh báo
        }
    }

    void SpawnRock()
    {
        if (realRockPrefab != null)
        {
            // Sinh ra cục đá thật ở tít trên cao, thẳng hàng với vòng tròn này
            Vector3 dropPos = transform.position + Vector3.up * spawnHeight;
            Instantiate(realRockPrefab, dropPos, Quaternion.identity);
        }
    }
}