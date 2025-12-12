using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Sức mạnh")]
    [SerializeField] private int damage = 1;
    [SerializeField] private string targetTag = "Monster"; // Chỉ đánh kẻ địch

    private void OnTriggerEnter2D(Collider2D other)
    {
        // In ra tên mọi thứ mà kiếm chạm vào để kiểm tra
        Debug.Log("Kiếm đã chạm vào: " + other.name);

        if (other.CompareTag(targetTag))
        {
            Debug.Log("--> Trúng kẻ địch rồi!"); // <--- Nếu hiện dòng này là Tag đúng

            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Debug.Log("--> Nhưng nó không có script Máu (EnemyHealth)!");
            }
        }
    }
}
