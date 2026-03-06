using UnityEngine;

public class EnemySkeleton : EnemyAI
{
    [Header("Skeleton Settings")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private int swordDamage = 20;

    protected override void PerformAttack()
    {
        if (playerTransform != null && hitPoint != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            hitPoint.position = transform.position + (Vector3)(direction * 0.8f);
        }
        animator.SetTrigger("Attack");
    }
    public void AnimationEvent_DealDamage()
    {
        Debug.Log("1. Event đã chạy!"); 

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