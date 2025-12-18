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
    [SerializeField] private float startAttackDelay = 0.1f; // Thời gian chờ kiếm vung tới mục tiêu
    [SerializeField] private float attackCooldown = 0.5f;   // Thời gian nghỉ sau khi đánh
    [SerializeField] private float attackOffset = 0.8f;

    private bool isAttacking = false;
    private Vector2 localDirection = Vector2.right;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ... (Giữ nguyên Input di chuyển) ...
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // [FIX LỖI] Phân biệt rõ Trái/Phải
        if (moveX != 0 || moveY != 0)
        {
            if (moveX > 0) localDirection = Vector2.right;
            else if (moveX < 0) localDirection = Vector2.left; // Thêm dòng này

            if (moveY > 0) localDirection = Vector2.up;
            else if (moveY < 0) localDirection = Vector2.down;
        }

        // Cập nhật vị trí AttackPoint
        if (attackPoint != null)
        {
            attackPoint.localPosition = localDirection * attackOffset;
        }

        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // --- LOGIC ĐÃ SỬA ---
    private void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Nếu KHÔNG trúng ai thì thôi
        if (hitEnemies.Length == 0) return;

        // --- 1. ƯU TIÊN PHÁT TIẾNG ĐỘNG NGAY (SOUND FIRST) ---
        // Phát ngay khi phát hiện có va chạm, TRƯỚC khi tính toán máu me
        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit, 0.5f);

        // --- 2. SAU ĐÓ MỚI TRỪ MÁU (VISUAL/LOGIC) ---
        foreach (Collider2D enemy in hitEnemies)
        {
            // Tính toán bạo kích
            bool isCrit = Random.Range(0f, 100f) < critChance;
            int finalDamage = baseDamage;
            if (isCrit) finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);

            // Gây sát thương
            EnemyHealth eHealth = enemy.GetComponent<EnemyHealth>();
            if (eHealth != null)
            {
                eHealth.TakeDamage(finalDamage);
            }

            // Hiện số damage
            if (DamagePopupGenerator.Instance != null)
            {
                DamagePopupGenerator.Instance.Create(enemy.transform.position, finalDamage, isCrit);
            }
        }
    }

    // Sửa lại AttackRoutine để gọi hàm void bên trên
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null)
        {
            // [TỐI ƯU] Dùng localDirection thay vì Input để đảm bảo
            // Animation chém cùng hướng với AttackPoint (Hitbox)
            animator.SetFloat("InputX", localDirection.x);
            animator.SetFloat("InputY", localDirection.y);
            animator.SetTrigger("Attack");
        }

        // Tiếng vung kiếm (Ngay lập tức)
        AudioManager.Instance.PlaySFX(AudioManager.Instance.swordSwing, 0.5f);

        // Chờ kiếm chạm quái
        yield return new WaitForSeconds(startAttackDelay);

        // Gọi hàm xử lý va chạm (Trong này sẽ phát tiếng Bụp -> rồi mới trừ máu)
        PerformAttack();

        // Hồi chiêu
        yield return new WaitForSeconds(Mathf.Max(0, attackCooldown - startAttackDelay));
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}