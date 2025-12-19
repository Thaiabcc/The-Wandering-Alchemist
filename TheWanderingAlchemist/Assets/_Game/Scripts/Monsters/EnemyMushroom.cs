using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMushroom : EnemyAI
{
    [Header("Mushroom Setting")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private int hitDame = 10;
    // [GỢI Ý] Mushroom tay ngắn hơn Skeleton nên offset nhỏ hơn
    [SerializeField] private float attackOffset = 0.5f;

    // Hàm tấn công
    protected override void PerformAttack()
    {
        if (playerTransform != null && hitPoint != null)
        {
            // --- [FIX LỖI: ĐỨNG TRÊN ĐẦU KHÔNG TRÚNG] ---

            // 1. Xác định tâm của con Nấm (Giả sử tâm cách chân 0.5f)
            // Bro có thể chỉnh số 0.5f này to/nhỏ tùy độ cao con quái
            Vector2 myCenter = (Vector2)transform.position + new Vector2(0, 1f);

            // 2. Xác định tâm của Player (Cũng nâng lên tương tự)
            Vector2 targetCenter = (Vector2)playerTransform.position + new Vector2(0, 1f);

            // 3. Tính hướng từ BỤNG đến BỤNG (Thay vì chân đến chân)
            Vector2 direction = (targetCenter - myCenter).normalized;

            // 4. Đặt HitBox xuất phát từ BỤNG + Hướng đánh
            hitPoint.position = myCenter + (direction * attackOffset);

            // ---------------------------------------------

            animator.SetTrigger("Attack");
        }
    }
    // Hàm này phải gắn vào Animation Event
    public void AnimationEvent_DealDame()
    {
        if (hitPoint == null) return;

        // [SỬA 2] Thêm Lọc Layer "Player" (Quan trọng!)
        // Để tránh chém trúng sàn nhà (Map) hoặc tường
        int playerLayer = LayerMask.GetMask("Player");

        // [SỬA 3] Check nếu chưa tạo Layer thì quét tất cả (chống lỗi)
        if (playerLayer == 0) playerLayer = -1;

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, playerLayer);

        foreach (var hit in hits)
        {
            // Kiểm tra Tag cho chắc chắn
            if (hit.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(hitDame);
                    Debug.Log("Nấm đấm trúng Player!");

                    // [SỬA 4] Return ngay lập tức
                    // Để tránh trường hợp Player có 2 collider (chân + người) thì bị trừ máu 2 lần
                    return;
                }
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected(); // Vẽ vòng tròn vàng/đỏ của cha
        if (hitPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }
    }
}