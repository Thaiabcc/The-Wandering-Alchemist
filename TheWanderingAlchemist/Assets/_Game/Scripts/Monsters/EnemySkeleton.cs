using UnityEngine;

// Kế thừa từ EnemyAI
public class EnemySkeleton : EnemyAI
{
    [Header("Skeleton Settings")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private int swordDamage = 20;

    // Ghi đè hàm tấn công của cha (EnemyAI)
    protected override void PerformAttack()
    {
        // [MỚI] Tự động di chuyển HitPoint về phía Player trước khi chém
        if (playerTransform != null && hitPoint != null)
        {
            // 1. Tính hướng từ Skeleton tới Player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // 2. Đặt HitPoint cách người Skeleton một đoạn (ví dụ 0.8f) về hướng đó
            // Bro có thể chỉnh số 0.8f này cho vừa với tầm vung kiếm
            hitPoint.position = transform.position + (Vector3)(direction * 0.8f);
        }

        // Chạy animation chém như cũ
        animator.SetTrigger("Attack");
    }

    // Hàm gọi từ Animation Event
    public void AnimationEvent_DealDamage()
    {
        Debug.Log("1. Event đã chạy!"); // Nếu dòng này không hiện -> Lỗi Bước 1 (Chưa gắn Event)

        if (hitPoint == null) return;

        // Quét xung quanh
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius);

        foreach (var hit in hits)
        {
            if (hit.GetComponent<PlayerStats>())
            {
                hit.GetComponent<PlayerStats>().TakeDamage(swordDamage);
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (hitPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }
    }
}