using UnityEngine;

public class RockWarning : MonoBehaviour
{
    [Header("--- CẤU HÌNH ---")]
    // [THAY ĐỔI]: Đổi thành mảng [] để chứa nhiều loại đá
    public GameObject[] rockPrefabs;

    public float warningDuration = 0.8f;
    public float spawnHeight = 10f;

    private float timer;

    void Start()
    {
        timer = warningDuration;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Hiệu ứng vòng tròn to dần
        float scale = 1f - (timer / warningDuration);
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 0.5f, 1f), scale);

        if (timer <= 0)
        {
            SpawnRock();
            Destroy(gameObject);
        }
    }

    void SpawnRock()
    {
        // Kiểm tra xem có cục đá nào trong danh sách không
        if (rockPrefabs != null && rockPrefabs.Length > 0)
        {
            // [THAY ĐỔI]: Chọn ngẫu nhiên 1 chỉ số từ 0 đến độ dài mảng
            int randomIndex = Random.Range(0, rockPrefabs.Length);

            GameObject rockToSpawn = rockPrefabs[randomIndex];

            if (rockToSpawn != null)
            {
                Vector3 dropPos = transform.position + Vector3.up * spawnHeight;
                Instantiate(rockToSpawn, dropPos, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("Chưa kéo Prefab đá vào RockWarning kìa bro!");
        }
    }
}