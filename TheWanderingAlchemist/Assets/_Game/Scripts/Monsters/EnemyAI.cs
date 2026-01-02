using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    #region Configuration
    [Header("Movement & Range")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown = 1.5f;

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolRadius = 3f;
    [SerializeField] protected float waitTime = 2f;
    [SerializeField] protected float stoppingDistance = 0.2f;

    [Header("Environment")]
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected float combatCenterOffset = 0.5f;
    #endregion

    #region Component References
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D mainCollider;
    #endregion

    #region State Variables
    public bool isDead { get; protected set; } = false;
    protected float lastAttackTime;

    // Patrol State
    protected Vector2 startPosition;
    protected Vector2 patrolTarget;
    protected float waitTimer;
    protected bool isWaiting = false;

    // Stuck Check
    private Vector2 lastPosition;
    private float stuckTimer;
    private const float STUCK_THRESHOLD_TIME = 0.5f;
    private const float MIN_MOVE_DISTANCE = 0.01f;
    #endregion

    #region Unity Lifecycle
    protected virtual void Start()
    {
        InitializeComponents();
        FindPlayer();

        startPosition = transform.position;
        lastPosition = transform.position;

        PickNewPatrolPoint();
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;

        // Nếu mất dấu player hoặc player chết -> Tuần tra
        if (playerTransform == null)
        {
            Patroling();
            return;
        }

        float distanceToPlayer = GetCombatDistanceToPlayer();

        if (distanceToPlayer < chaseRange)
        {
            Chasing(distanceToPlayer);
        }
        else
        {
            Patroling();
        }
    }
    #endregion

    #region Core Logic (Overridable)

    protected virtual void Chasing(float distance)
    {
        ResetPatrolState(); // Reset timer wait/stuck khi đang đuổi

        if (distance <= attackRange)
        {
            HandleCombat();
        }
        else
        {
            MoveTo(playerTransform.position);
        }
    }

    protected virtual void Patroling()
    {
        float distanceToTarget = Vector2.Distance(transform.position, patrolTarget);

        if (distanceToTarget < stoppingDistance)
        {
            HandlePatrolWait();
        }
        else
        {
            if (!isWaiting)
            {
                MoveTo(patrolTarget);
                CheckIfStuck();
            }
        }
    }

    protected virtual void PerformAttack()
    {
        // Class con sẽ override logic này (gây damage, play sound, v.v.)
    }

    #endregion

    #region Helper Methods

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCollider = GetComponent<Collider2D>();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    // Tính khoảng cách dựa trên Offset ngực/tâm thay vì chân
    protected float GetCombatDistanceToPlayer()
    {
        Vector2 myCenter = GetCombatCenter(transform);
        Vector2 targetCenter = GetCombatCenter(playerTransform);
        return Vector2.Distance(myCenter, targetCenter);
    }

    protected Vector2 GetCombatCenter(Transform t)
    {
        return (Vector2)t.position + new Vector2(0, combatCenterOffset);
    }

    private void HandleCombat()
    {
        StopMoving();
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack();
        }
    }

    private void HandlePatrolWait()
    {
        StopMoving();
        stuckTimer = 0; // Reset stuck khi đã đến đích

        if (!isWaiting) // Bắt đầu chờ
        {
            waitTimer = waitTime;
            isWaiting = true;
        }
        else // Đang chờ
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0)
            {
                PickNewPatrolPoint();
                isWaiting = false;
            }
        }
    }

    private void CheckIfStuck()
    {
        // Kiểm tra quãng đường di chuyển được so với frame trước
        float movedDistance = Vector2.Distance(transform.position, lastPosition);
        float expectedMinDistance = MIN_MOVE_DISTANCE * moveSpeed * Time.fixedDeltaTime * 5f;

        if (movedDistance < expectedMinDistance)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer > STUCK_THRESHOLD_TIME)
            {
                PickNewPatrolPoint(); // Kẹt quá lâu -> đổi điểm
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0; // Di chuyển tốt
        }

        lastPosition = transform.position;
    }

    protected void ResetPatrolState()
    {
        waitTimer = 0;
        isWaiting = false;
        stuckTimer = 0;
    }

    protected void PickNewPatrolPoint()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle * patrolRadius;
            Vector2 potentialTarget = startPosition + randomDir;

            if (IsPathClear(potentialTarget))
            {
                patrolTarget = potentialTarget;
                return;
            }
        }
        // Fallback: Đứng yên nếu không tìm được điểm
        patrolTarget = transform.position;
    }

    private bool IsPathClear(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPos);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);
        return hit.collider == null;
    }

    #endregion

    #region Movement & Action
    protected void MoveTo(Vector2 target)
    {
        animator.SetBool("isMoving", true);
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + (dir * moveSpeed * Time.fixedDeltaTime));
        FlipSprite(target);
    }

    protected void StopMoving()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    protected void FlipSprite(Vector2 target)
    {
        // So sánh x để lật mặt
        spriteRenderer.flipX = target.x < transform.position.x;
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        StopMoving();
        rb.isKinematic = true;
        animator.SetBool("Dead", true);

        if (mainCollider != null) mainCollider.enabled = false;
    }
    #endregion

    #region Debugging
    protected virtual void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + new Vector3(0, combatCenterOffset, 0);

        // Combat Ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center, 0.1f);

        // Patrol Area
        Gizmos.color = Color.green;
        Vector3 patrolCenter = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Gizmos.DrawWireSphere(patrolCenter, patrolRadius);

        // Target Line
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, patrolTarget);
            Gizmos.DrawSphere(patrolTarget, 0.2f);
        }
    }
    #endregion

}