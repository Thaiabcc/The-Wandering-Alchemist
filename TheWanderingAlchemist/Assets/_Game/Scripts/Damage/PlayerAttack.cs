using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [Header("Cài đặt")]
    [SerializeField] private GameObject attackPoint; // Kéo cái AttackPoint tàng hình vào đây
    [SerializeField] private Animator animator;      // Kéo Animator của Player vào
    [SerializeField] private float attackTime = 0.3f; // Thời gian gây sát thương (chỉnh cho khớp animation)
    [SerializeField] private float attackCooldown = 0.5f;

    private bool isAttacking = false;
    private void Awake()
    {
        // Tự động tìm Animator trên chính object này
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // Tắt vùng sát thương ngay từ đầu cho chắc
         if (attackPoint != null) attackPoint.SetActive(false);
    }

    private void Update()
    {
        // Chặn không cho spam nút khi đang đánh
        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. Chạy Animation (Phần Nhìn)
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 2. Bật vùng sát thương (Phần Dame)
        // Mẹo: Nếu animation vung tay chậm, bạn có thể thêm yield return new WaitForSeconds(0.1f) ở đây để delay
        if (attackPoint != null)
        {
            attackPoint.SetActive(true);
        }

        // 3. Giữ vùng sát thương trong thời gian vung gậy
        yield return new WaitForSeconds(attackTime);

        // 4. Tắt vùng sát thương
        if (attackPoint != null)
        {
            attackPoint.SetActive(false);
        }

        // 5. Chờ hồi chiêu (để animation kịp quay về Idle)
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
}
