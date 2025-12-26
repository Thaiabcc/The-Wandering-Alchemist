using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    [Header("Cài đặt Phần Thưởng")]
    public GameObject goldPrefab; // Kéo Prefab vàng vào đây
    public int goldAmount = 1;    // Số lượng vàng rớt ra (nếu muốn rớt nhiều cục)
    public float popForce = 3f;   // Lực bắn vàng lên cao

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isOpened)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        anim.SetTrigger("Open");
        Debug.Log("Rương đã mở!");

        // --- PHẦN TẠO VÀNG ---
        if (goldPrefab != null)
        {
            SpawnLoot();
        }
        else
        {
            Debug.LogWarning("Chưa gắn Prefab vàng vào rương kìa bro!");
        }

        // Xóa rương sau 0.5 giây
        Destroy(gameObject, 0.5f);
    }

    void SpawnLoot()
    {
        for (int i = 0; i < goldAmount; i++)
        {
            // 1. Tạo cục vàng ngay tại vị trí rương
            GameObject loot = Instantiate(goldPrefab, transform.position, Quaternion.identity);

            // 2. Lấy Rigidbody2D của cục vàng để thêm lực đẩy
            Rigidbody2D rb = loot.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Tạo hướng ngẫu nhiên (bắn lên trên và hơi lệch sang trái/phải)
                Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;

                // Thêm lực bắn (Impulse = lực tức thời)
                rb.AddForce(dropDirection * popForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Nhấn E để mở rương");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}