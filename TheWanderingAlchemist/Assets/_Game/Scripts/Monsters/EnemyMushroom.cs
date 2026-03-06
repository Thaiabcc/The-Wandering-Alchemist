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
        if (playerTransform != null && hitPoint != null)
        {

            Vector2 myCenter = (Vector2)transform.position + new Vector2(0, 1f);
            Vector2 targetCenter = (Vector2)playerTransform.position + new Vector2(0, 1f);
            Vector2 direction = (targetCenter - myCenter).normalized;
            hitPoint.position = myCenter + (direction * attackOffset);

            // ---------------------------------------------

            animator.SetTrigger("Attack");
        }
    }
    public void AnimationEvent_DealDame()
    {
        if (hitPoint == null) return;
        int playerLayer = LayerMask.GetMask("Player");
        if (playerLayer == 0) playerLayer = -1;

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(hitDame);
                    Debug.Log("Nấm đấm trúng Player!");
                    return;
                }
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