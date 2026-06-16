using UnityEngine;

public class EnemySkeleton : EnemyAI
{
    [Header("Skeleton Settings")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private int swordDamage = 20;

    protected override void PerformAttack()
    {
        FlipSprite(playerTransform.position);
    
        if (playerTransform != null && hitPoint != null)
        {
            float offset = spriteRenderer.flipX ? -0.8f : 0.8f;
            hitPoint.localPosition = new Vector3(offset, 0, 0);
        }
        animator.SetTrigger("Attack");
    }
    public void AnimationEvent_DealDamage()
    {
        if (hitPoint == null) return;
        int playerLayer = LayerMask.GetMask("Player");
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;
            if (hit.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.TakeDamage(swordDamage);
                Debug.Log("Skeleton gây dame: " + swordDamage);
                break; 
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