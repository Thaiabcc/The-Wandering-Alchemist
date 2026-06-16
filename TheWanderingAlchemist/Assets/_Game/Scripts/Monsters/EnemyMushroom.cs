using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMushroom : EnemyAI
{
    [Header("Mushroom Setting")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private int hitDame = 10;
    [SerializeField] private float attackOffset = 0.5f;

    protected override void PerformAttack()
    {
        FlipSprite(playerTransform.position);
        float directionMultiplier = spriteRenderer.flipX ? -1f : 1f;
        hitPoint.localPosition = new Vector3(Mathf.Abs(hitPoint.localPosition.x) * directionMultiplier, hitPoint.localPosition.y, 0);

        animator.SetTrigger("Attack");
    }
    public void AnimationEvent_DealDame()
    {
        Collider2D hit = Physics2D.OverlapCircle(hitPoint.position, hitRadius, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            if (hit.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.TakeDamage(hitDame);
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