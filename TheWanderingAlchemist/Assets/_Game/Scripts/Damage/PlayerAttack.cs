using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Cài đặt")]
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private Animator animator;

    [Header("Tinh chỉnh Thời gian")]
    [SerializeField] private float startAttackDelay = 0.1f; // <--- MỚI: Chờ bao lâu thì mới bật Hitbox?
    [SerializeField] private float attackTime = 0.2f;       // Hitbox tồn tại trong bao lâu?
    [SerializeField] private float attackCooldown = 0.5f;   // Hồi chiêu

    [SerializeField] private float attackOffset = 0.8f;

    private bool isAttacking = false;
    private Vector2 lastDirection = Vector2.right;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (attackPoint != null) attackPoint.SetActive(false);
    }

    private void Update()
    {
        // ... (Giữ nguyên đoạn cập nhật hướng đánh moveX, moveY)
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
            attackPoint.transform.localPosition = lastDirection * attackOffset;
        }
        // ...

        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. Chạy Animation trước
        if (animator != null)
        {
            animator.SetFloat("InputX", lastDirection.x);
            animator.SetFloat("InputY", lastDirection.y);
            animator.SetTrigger("Attack");
        }

        // 2. --- CHỜ MỘT XÍU (Delay) ---
        // Để animation kịp vung tay lên cao rồi mới bật sát thương
        yield return new WaitForSeconds(startAttackDelay);

        // 3. BẬT SÁT THƯƠNG
        if (attackPoint != null) attackPoint.SetActive(true);

        // 4. GIỮ SÁT THƯƠNG (Trong bao lâu)
        yield return new WaitForSeconds(attackTime);

        // 5. TẮT SÁT THƯƠNG
        if (attackPoint != null) attackPoint.SetActive(false);

        // 6. HỒI CHIÊU
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}