using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class EnemyAI : MonoBehaviour
{
    [Header("Cài đặt Hành vi")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 5f;

    [Header("Tấn công")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float stopDistance = 0.5f;

    // --- [XÓA] Bỏ hết phần Máu cũ ở đây đi nhé ---

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Biến để khóa hành động khi đang diễn cảnh chết
    public bool isDead { get; private set; } = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void FixedUpdate()
    {
        // Nếu chết hoặc không thấy player thì thôi
        if (isDead || playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // TRƯỜNG HỢP 1: Trong tầm nhìn đuổi theo (chaseRange)
        if (distanceToPlayer < chaseRange)
        {
            // [LOGIC MỚI] Chỉ di chuyển nếu khoảng cách VẪN LỚN HƠN khoảng cách dừng
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
                animator.SetBool("isMoving", true); // Đi thì Walk
                FlipSprite();
            }
            else
            {
                // [FIX LỖI] Đã đến sát bên rồi -> Dừng lại -> Về Idle
                rb.velocity = Vector2.zero;
                animator.SetBool("isMoving", false); // Đứng im thì Idle

                // (Sau này bạn có thể chèn code tấn công ở đây: Attack())
            }
        }
        // TRƯỜNG HỢP 2: Ngoài tầm nhìn -> Đứng chơi
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
        }
    }

    // --- [MỚI] Hàm này sẽ được gọi bởi EnemyHealth ---
    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        // 1. Dừng vật lý
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // 2. Tắt isMoving (để thỏa mãn điều kiện 1)
        animator.SetBool("isMoving", false);

        // 3. [THAY ĐỔI LỚN] Bật Bool "dead" lên (để chặn đường về Idle)
        animator.SetBool("Dead", true);
        // (Lưu ý: "dead" là tên biến Bool bạn vừa tạo trong Animator)

        // 4. Tắt va chạm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.fixedDeltaTime));
    }

    private void FlipSprite()
    {
        if (playerTransform.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Chết rồi không cắn được

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null) playerStats.TakeDamage(damage);

            PlayerMovement playerMove = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMove != null)
            {
                Vector2 pushDir = (collision.transform.position - transform.position).normalized;
                playerMove.ApplyKnockback(pushDir, knockbackForce, 0.2f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}