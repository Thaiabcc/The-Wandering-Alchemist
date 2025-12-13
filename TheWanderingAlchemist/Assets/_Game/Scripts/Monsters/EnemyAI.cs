using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Cài đặt Hành vi")]
    [SerializeField] private float moveSpeed = 2f;      // Tốc độ chạy (chậm hơn Player xíu)
    [SerializeField] private float chaseRange = 5f;     // Tầm nhìn (Gần bao nhiêu thì bắt đầu đuổi)

    [Header("Tấn công")]
    [SerializeField] private int damage = 1;            // Cắn đau thế nào
    [SerializeField] private float knockbackForce = 5f; // Đẩy lùi người chơi khi cắn

    private Transform playerTransform; // Vị trí người chơi
    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tự tìm thằng Player trong map để ghim mục tiêu
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        // 1. Tính khoảng cách tới người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // 2. Nếu trong tầm nhìn -> Đuổi theo
        if (distanceToPlayer < chaseRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Nếu xa quá thì đứng chơi (Stop)
            rb.velocity = Vector2.zero;
        }
    }

    private void MoveTowardsPlayer()
    {
        // Tính hướng lao tới
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Di chuyển bằng Rigidbody (để giữ tính vật lý)
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.fixedDeltaTime));
    }

    // 3. Xử lý va chạm (CẮN NGƯỜI CHƠI)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu cái thứ mình vừa húc vào là Player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                // Cắn 1 phát
                playerStats.TakeDamage(damage);

                // Đẩy lùi người chơi ra (Knockback) để không bị cắn liên tục
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Tính hướng đẩy (ngược lại hướng va chạm)
                    Vector2 pushDir = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    // Vẽ vòng tròn tầm nhìn trong Editor để dễ chỉnh (Gizmos)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
