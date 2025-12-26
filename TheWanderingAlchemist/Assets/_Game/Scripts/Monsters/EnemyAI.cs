using UnityEngine;

// Class Cha: Chứa logic chung (Di chuyển, Tuần tra, Đuổi theo)
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    [Header("Chỉ số Chung")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float attackRange = 1f;       // Tầm đánh
    [SerializeField] protected float attackCooldown = 1.5f;  // Hồi chiêu

    [Header("Cài đặt Tuần tra")]
    [SerializeField] protected float patrolRadius = 3f;
    [SerializeField] protected float waitTime = 2f;

    // [MỚI] Layer để nhận biết đâu là tường/vật cản. NHỚ CHỌN LAYER NÀY TRONG INSPECTOR!
    [Header("Cài đặt Môi trường (QUAN TRỌNG)")]
    [SerializeField] protected LayerMask obstacleLayer;

    [Header("Tinh chỉnh tâm đánh")]
    // [MỚI] Biến này để chỉnh tâm vòng tròn lên cao (ví dụ 0.5 hoặc 1.0)
    [SerializeField] protected float combatCenterOffset = 0.5f;

    // Các biến tham chiếu (Protected để con dùng được)
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    // Trạng thái
    public bool isDead { get; protected set; } = false; // Public get để script khác check
    protected float lastAttackTime;

    // Biến tuần tra
    protected Vector2 startPosition;
    protected Vector2 patrolTarget;
    protected float waitTimer;
    protected bool isWaiting = false;

    // [MỚI] Biến dùng để check xem có bị kẹt không
    private Vector2 lastPosition;
    private float stuckTimer;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        startPosition = transform.position;
        lastPosition = transform.position; // Khởi tạo vị trí cũ

        PickNewPatrolPoint();
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;

        if (playerTransform == null)
        {
            Patroling();
            return;
        }

        // --- [SỬA LẠI: TỰ TÍNH TÂM (OFFSET) THAY VÌ DỰA VÀO COLLIDER] ---

        // 1. Lấy vị trí chân + nâng lên một đoạn (combatCenterOffset)
        Vector2 myCombatCenter = (Vector2)transform.position + new Vector2(0, combatCenterOffset);

        // 2. Làm tương tự với Player (Giả sử Player cũng cần tính từ ngực)
        Vector2 targetCombatCenter = (Vector2)playerTransform.position + new Vector2(0, combatCenterOffset);

        // 3. Tính khoảng cách giữa 2 ĐIỂM NGỰC
        float distance = Vector2.Distance(myCombatCenter, targetCombatCenter);

        // ----------------------------------------

        if (distance < chaseRange)
        {
            Chasing(distance);
        }
        else
        {
            Patroling();
        }
    }

    protected virtual void Chasing(float distance)
    {
        waitTimer = 0;
        isWaiting = false;
        stuckTimer = 0; // Khi đuổi theo thì reset bộ đếm kẹt

        if (distance <= attackRange)
        {
            StopMoving();
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                PerformAttack(); // Gọi hàm tấn công ảo
            }
        }
        else
        {
            MoveTo(playerTransform.position);
        }
    }

    protected virtual void Patroling()
    {
        float distanceToTarget = Vector2.Distance(transform.position, patrolTarget);

        if (distanceToTarget < 0.2f)
        {
            StopMoving();
            stuckTimer = 0; // Đến nơi rồi thì reset kẹt

            if (waitTimer <= 0)
            {
                waitTimer = waitTime;
                isWaiting = true;
            }
            else
            {
                waitTimer -= Time.fixedDeltaTime;
                if (waitTimer <= 0)
                {
                    PickNewPatrolPoint();
                    isWaiting = false;
                }
            }
        }
        else
        {
            if (!isWaiting)
            {
                MoveTo(patrolTarget);

                // --- [MỚI] LOGIC CHỐNG KẸT (STUCK CHECK) ---
                // Nếu đang cố đi mà vị trí không thay đổi nhiều -> Đang húc tường
                if (Vector2.Distance(transform.position, lastPosition) < 0.01f * moveSpeed * Time.fixedDeltaTime * 5f)
                {
                    stuckTimer += Time.fixedDeltaTime;

                    // Nếu đứng im quá 0.5 giây -> Chọn điểm mới ngay
                    if (stuckTimer > 0.5f)
                    {
                        PickNewPatrolPoint();
                        stuckTimer = 0;
                    }
                }
                else
                {
                    stuckTimer = 0; // Đang đi tốt
                }

                lastPosition = transform.position; // Lưu vị trí frame này
            }
        }
    }

    // --- HÀM ẢO (VIRTUAL) ĐỂ CON GHI ĐÈ ---
    protected virtual void PerformAttack()
    {
        // Mặc định để trống, con nào cần animation thì override lại
    }

    // --- CÁC HÀM HỖ TRỢ ---
    protected void MoveTo(Vector2 target)
    {
        animator.SetBool("isMoving", true);
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.MovePosition((Vector2)transform.position + (dir * moveSpeed * Time.fixedDeltaTime));
        FlipSprite(target);
    }

    protected void StopMoving()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    // [MỚI] Hàm chọn điểm tuần tra thông minh hơn
    protected void PickNewPatrolPoint()
    {
        // Thử tìm điểm hợp lệ tối đa 10 lần
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle * patrolRadius;
            Vector2 potentialTarget = startPosition + randomDir;

            // Bắn tia từ chân quái đến điểm dự kiến để xem có tường chắn không
            Vector2 direction = (potentialTarget - (Vector2)transform.position).normalized;
            float distance = Vector2.Distance(transform.position, potentialTarget);

            // Kiểm tra va chạm với lớp Obstacle
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);

            // Nếu không va vào tường (hit.collider == null) -> Điểm này OK
            if (hit.collider == null)
            {
                patrolTarget = potentialTarget;
                return;
            }
        }

        // Nếu thử 10 lần mà vẫn vướng tường, đứng yên tại chỗ chờ lượt sau
        patrolTarget = transform.position;
    }

    protected void FlipSprite(Vector2 target)
    {
        if (target.x < transform.position.x) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        animator.SetBool("isMoving", false);
        animator.SetBool("Dead", true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Tính tâm vẽ dựa trên offset
        Vector3 center = transform.position + new Vector3(0, combatCenterOffset, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);

        // Vẽ thêm 1 chấm nhỏ để biết tâm đang ở đâu
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center, 0.1f);

        // Vẽ vùng tuần tra (Patrol) vẫn lấy chân làm gốc
        Gizmos.color = Color.green;
        Vector3 patrolCenter = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Gizmos.DrawWireSphere(patrolCenter, patrolRadius);

        // Vẽ điểm đang định đi tới
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, patrolTarget);
            Gizmos.DrawSphere(patrolTarget, 0.2f);
        }
    }
}