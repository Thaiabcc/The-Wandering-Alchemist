using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Cài đặt")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;

    [Header("Chỉ số Chiến đấu")]
    [SerializeField] private int baseDamage = 100;
    [SerializeField] private float critChance = 30f;
    [SerializeField] private float critMultiplier = 2f;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Tinh chỉnh Thời gian")]
    [SerializeField] private float startAttackDelay = 0.1f;
    [SerializeField] private float attackCooldown = 0.5f;

    // [QUAN TRỌNG] Biến này dùng để chỉnh vị trí Hitbox
    [SerializeField] private float attackOffset = 0.8f;

    private bool isAttacking = false;

    // Mặc định luôn là (1,0) vì Rotation của PlayerMovement đã lo việc xoay hướng
    private Vector2 localDirection = Vector2.right;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Đọc Input để chỉnh hướng đánh (Nếu game bạn đánh được lên/xuống)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // --- SỬA LOGIC XUNG ĐỘT TẠI ĐÂY ---
        // Nếu PlayerMovement đã dùng Rotation để xoay trái/phải
        // Thì ở đây chúng ta LUÔN coi hướng ngang là bên Phải (Vector2.right)
        // Vì khi cha xoay 180 độ, Vector2.right của con sẽ tự thành bên Trái thế giới.

        if (moveX != 0 || moveY != 0)
        {
            if (moveX != 0)
            {
                // Dù đi trái hay phải, Hitbox vẫn luôn nằm ở "phía trước mặt" (Local X dương)
                localDirection = Vector2.right;
            }

            // Nếu có đánh lên xuống thì giữ nguyên logic Y
            if (moveY > 0) localDirection = Vector2.up;
            else if (moveY < 0) localDirection = Vector2.down;
        }

        // Cập nhật vị trí Hitbox theo hướng Local
        if (attackPoint != null)
        {
            // Bây giờ đi trái -> localDirection vẫn là Right -> AttackPoint nằm ở X dương (trước mặt)
            // Nhờ PlayerMovement xoay 180 độ -> Trước mặt lúc này chính là bên Trái! -> CHUẨN BÀI
            attackPoint.localPosition = localDirection * attackOffset;
        }
        // -----------------------------------

        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null)
        {
            // Gửi thông số vào Animator (InputX, InputY)
            // Lưu ý: Animator có thể cần giá trị thực tế của MoveX để biết đang chạy hay đứng
            // Nhưng blend tree đánh nhau thì thường dựa vào hướng mặt
            animator.SetFloat("InputX", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("InputY", Input.GetAxisRaw("Vertical"));
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(startAttackDelay);

        PerformAttack();

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            bool isCrit = Random.Range(0f, 100f) < critChance;
            int finalDamage = baseDamage;

            if (isCrit) finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);

            // Kiểm tra null để tránh lỗi đỏ lòm console nếu quái chưa có máu
            EnemyHealth eHealth = enemy.GetComponent<EnemyHealth>();
            if (eHealth != null)
            {
                eHealth.TakeDamage(finalDamage);
            }

            // Debug.Log("Đánh trúng " + enemy.name + " Damage: " + finalDamage);

            // Nếu có Singleton Popup thì gọi, không thì thôi tránh lỗi
            if (DamagePopupGenerator.Instance != null)
            {
                DamagePopupGenerator.Instance.Create(enemy.transform.position, finalDamage, isCrit);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}