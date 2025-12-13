using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Cài đặt")]
    [SerializeField] private Transform attackPoint; // Đổi từ GameObject sang Transform cho nhẹ
    [SerializeField] private Animator animator;

    // --- MỚI: Cài đặt chỉ số chiến đấu ---
    [Header("Chỉ số Chiến đấu")]
    [SerializeField] private int baseDamage = 100;      // Sát thương gốc
    [SerializeField] private float critChance = 30f;    // Tỉ lệ bạo kích (30%)
    [SerializeField] private float critMultiplier = 2f; // Nhân đôi khi bạo kích
    [SerializeField] private float attackRange = 1.0f;  // Phạm vi đánh
    [SerializeField] private LayerMask enemyLayers;     // Chỉ đánh vào lớp Enemy

    [Header("Tinh chỉnh Thời gian")]
    [SerializeField] private float startAttackDelay = 0.1f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackOffset = 0.8f;

    private bool isAttacking = false;
    private Vector2 lastDirection = Vector2.right;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            if (moveX > 0) lastDirection = Vector2.right;
            else if (moveX < 0) lastDirection = Vector2.left;

            if (moveY > 0) lastDirection = Vector2.up;
            else if (moveY < 0) lastDirection = Vector2.down;
        }

        if (attackPoint != null)
        {
            attackPoint.localPosition = lastDirection * attackOffset;
        }

        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. Chạy Animation
        if (animator != null)
        {
            animator.SetFloat("InputX", lastDirection.x);
            animator.SetFloat("InputY", lastDirection.y);
            animator.SetTrigger("Attack");
        }

        // 2. Chờ vung tay (Delay)
        yield return new WaitForSeconds(startAttackDelay);

        // 3. --- MỚI: XÚC XẮC & GÂY SÁT THƯƠNG TẠI ĐÂY ---
        PerformAttack();

        // 4. Hồi chiêu
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // --- MỚI: Hàm xử lý logic đánh đấm ---
    private void PerformAttack()
    {
        // A. Tìm tất cả kẻ địch trong vùng tròn (Hitbox ảo)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // B. "Xúc xắc" tính bạo kích cho TỪNG con quái trúng đòn
            bool isCrit = Random.Range(0f, 100f) < critChance;

            // Tính damage cuối cùng
            int finalDamage = baseDamage;
            if (isCrit)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);
            }

            // C. Gửi sát thương cho Enemy (Giả sử enemy có script EnemyHealth)
            enemy.GetComponent<EnemyHealth>().TakeDamage(finalDamage);
            // Debug tạm để test nếu chưa có script EnemyHealth:
            Debug.Log("Đánh trúng " + enemy.name + " - Damage: " + finalDamage + " - Crit: " + isCrit);

            // D. HIỆN SỐ DAMAGE BAY LÊN (Gọi Đạo Diễn File 2)
            DamagePopupGenerator.Instance.Create(enemy.transform.position, finalDamage, isCrit);
        }
    }

    // --- MỚI: Vẽ vòng tròn đỏ trong Editor để dễ chỉnh tầm đánh ---
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}