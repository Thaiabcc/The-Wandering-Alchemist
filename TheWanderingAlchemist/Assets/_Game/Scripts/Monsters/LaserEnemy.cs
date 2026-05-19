using UnityEngine;

public class LaserEnemy : EnemyAI
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float damagePerSecond = 20f;
    [SerializeField] private LayerMask laserHitLayers;

    [Header("Visual Juice")]
    [SerializeField] private float minWidth = 0.05f;
    [SerializeField] private float maxWidth = 0.15f;
    [SerializeField] private float scrollSpeed = 4f;
    [SerializeField] private float jitterAmount = 0.02f;

    private float firePointInitialLocalX;
    private bool isTargeting = false;

    protected override void Start()
    {
        base.Start();
        if (firePoint != null) firePointInitialLocalX = Mathf.Abs(firePoint.localPosition.x);
    }

    protected override void FixedUpdate()
    {   
        base.FixedUpdate();

        if (isDead || playerTransform == null)
        {
            isTargeting = false;
            return;
        }

        float distance = GetCombatDistanceToPlayer();
        isTargeting = (distance <= attackRange);

        if (isTargeting)
        {
            HandleLaserDamage();
        }
    }

    private void LateUpdate()
    {
        if (isDead || !isTargeting || playerTransform == null)
        {
            StopLaser();
            return;
        }

        DrawLaser();
    }

    private void HandleLaserDamage()
    {
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, attackRange + 1f, laserHitLayers);

        if (hit != null && hit.collider != null && hit.collider.CompareTag("Player"))
        {
            hit.collider.SendMessage("TakeDamage", damagePerSecond * Time.fixedDeltaTime, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void DrawLaser()
    {
        if (!lineRenderer.enabled) lineRenderer.enabled = true;

        FlipSprite(playerTransform.position);
        
        Vector3 localPos = firePoint.localPosition;
        localPos.x = spriteRenderer.flipX ? -firePointInitialLocalX : firePointInitialLocalX;
        firePoint.localPosition = localPos;

        lineRenderer.material.mainTextureOffset -= new Vector2(Time.deltaTime * scrollSpeed, 0);

        float randomWidth = Random.Range(minWidth, maxWidth);
        lineRenderer.startWidth = randomWidth;
        lineRenderer.endWidth = randomWidth * 0.8f;

        lineRenderer.SetPosition(0, firePoint.position);

        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, attackRange + 1f, laserHitLayers);

        if (hit.collider != null)
        {
            Vector3 jitter = (Vector3)Random.insideUnitCircle * jitterAmount;
            lineRenderer.SetPosition(1, (Vector3)hit.point + jitter);
        }
        else
        {
            lineRenderer.SetPosition(1, (Vector2)firePoint.position + (direction * attackRange));
        }
    }

    private void StopLaser()
    {   
        if (lineRenderer != null && lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }
    }

    public override void TriggerDeath()
    {
        base.TriggerDeath();
        isTargeting = false;
        StopLaser();
    }
}