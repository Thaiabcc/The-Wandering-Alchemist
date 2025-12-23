using UnityEngine;

public class StaticDamage : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Kiểm tra xem va chạm với ai?
        if (hitInfo.CompareTag("Player"))
        {
            // 1. Tìm script PlayerStats trên người Player
            PlayerStats playerStats = hitInfo.GetComponent<PlayerStats>();

            // 2. Nếu tìm thấy thì trừ máu
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                Debug.Log("Đã gây dame cho Player: " + damage); // Debug xem chạy chưa
            }
        }
    }
}