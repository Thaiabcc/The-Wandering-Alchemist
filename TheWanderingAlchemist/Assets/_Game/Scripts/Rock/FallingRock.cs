using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public int damage = 15;
    public GameObject impactVFX; // Kéo Prefab hiệu ứng bụi/nổ vào đây

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 1. XOAY NGẪU NHIÊN: Tạo lực xoay để đá lăn lộn trên không
        float randomTorque = Random.Range(-300f, 300f);
        rb.angularVelocity = randomTorque;

        // 2. NẶNG HƠN: Tăng Gravity Scale lên 3 hoặc 4 trong Inspector cho rơi nhanh

        Destroy(gameObject, 5f); // Tự hủy nếu rơi xuống vực
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Chỉ nổ khi chạm Player hoặc Đất (Ground)
        if (hitInfo.CompareTag("Player") || hitInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Gây dame nếu trúng Player
            if (hitInfo.CompareTag("Player"))
            {
                var player = hitInfo.GetComponent<PlayerStats>(); // Hoặc script máu của bro
                if (player != null) player.TakeDamage(damage);
            }

            // TẠO HIỆU ỨNG BỤI (QUAN TRỌNG)
            if (impactVFX != null)
            {
                Instantiate(impactVFX, transform.position, Quaternion.identity);
            }

            // Hủy cục đá
            Destroy(gameObject);
        }
    }
}