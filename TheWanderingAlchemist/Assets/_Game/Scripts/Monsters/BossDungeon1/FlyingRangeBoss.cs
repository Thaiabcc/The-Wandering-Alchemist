using UnityEngine;
using System.Collections;

public class FlyingRangeBoss : EnemyAI
{
    [Header("Mov")]
    public float stopDistance = 5f;
    public float flyHeight = 2f;
    public float bobSpeed = 2f;

    [Header("Atta")]
    public Transform firePoint;
    public GameObject part1Prefab;
    public GameObject part2Prefab;
    public GameObject part3Prefab;
    public float segmentLength = 1.5f;
    public float spawnDelay = 0.1f;

    [Header("Tele")]
    public int teleportChance = 30;
    public float teleportDuration = 0.5f;
    public float teleportOffset = 3.0f;
    [Tooltip("Time warning")]
    public float postTeleportIdleTime = 1.0f;

    [Header("Rage")]
    public bool enableRage = true;
    public Color rageColor = Color.red;
    public GameObject rageVFX;
    public float chargeDuration = 3.0f;
    public GameObject rockPrefab; 
    public float rockSpawnRate = 0.2f;

    [Header("Poise")]
    public float maxPoise = 100f;
    public float poiseRecoveryRate = 5f;

    [Header("Conenc")]
    public BossHUD bossHUD;

    [Header("Patrol")]
    public Transform[] patrolPoints;

    private int currentPatrolIndex = 0;
    private float waypointWaitTimer = 0;

    private bool isRaging = false;
    private bool isCharging = false;
    private bool isTeleporting = false;
    private bool isStunned = false;
    private bool isFightStarted = false; 

    private float currentPoise;

    private SpriteRenderer mySprite;
    private Collider2D myCollider;
    private Vector3 originalScale;

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 0; 
        attackRange = stopDistance;
        currentPoise = maxPoise;

        mySprite = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;

        if (bossHUD != null)
        {
            bossHUD.gameObject.SetActive(false); 
        }
    }

    protected override void FixedUpdate()
    {
        if (isDead || playerTransform == null) return;

        if (!isStunned && currentPoise < maxPoise && !isCharging)
        {
            currentPoise += poiseRecoveryRate * Time.fixedDeltaTime;
            if (bossHUD != null) bossHUD.UpdatePoise(currentPoise);
        }

        if (isCharging || isTeleporting || isStunned)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance < chaseRange && !isFightStarted && CheckLineOfSight())
        {
            StartBossFight();
        }

        if (distance < chaseRange)
        {
            float face = (playerTransform.position.x > transform.position.x) ? 1 : -1;
            float currentScaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(currentScaleX * face, transform.localScale.y, 1);
        }

        if (isFightStarted && distance < chaseRange && CheckLineOfSight())
        {
            if (distance > stopDistance)
            {
                animator.SetBool("isMoving", true);

                Vector2 targetPos = playerTransform.position;
                targetPos.y += flyHeight + Mathf.Sin(Time.time * bobSpeed) * 0.5f; 

                rb.MovePosition(Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime));
            }
            else
            {
                animator.SetBool("isMoving", false);

                if (distance < stopDistance * 0.5f)
                {
                    Vector2 back = Vector2.MoveTowards(transform.position, playerTransform.position, -moveSpeed * Time.fixedDeltaTime);
                    rb.MovePosition(back);
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    DecideAttack();
                }
            }
        }
        else
        {
            Patroling();
        }
    }
    
    void StartBossFight()
    {
        isFightStarted = true;
        if (bossHUD != null)
            bossHUD.gameObject.SetActive(true);
    }
    
    protected override void Patroling()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * 0.01f;
            transform.position += new Vector3(0, bob, 0);
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        float dist = Vector2.Distance(transform.position, target.position);

        float face = (target.position.x > transform.position.x) ? 1 : -1;
        float currentScaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(currentScaleX * face, transform.localScale.y, 1);

        if (dist < 0.2f)
        {
            if (waypointWaitTimer <= 0)
            {
                waypointWaitTimer = waitTime;
            }
            else
            {
                waypointWaitTimer -= Time.deltaTime;
                if (waypointWaitTimer <= 0)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
        }
        else
        {
            animator.SetBool("isMoving", true);
            Vector2 next = Vector2.MoveTowards(transform.position, target.position, moveSpeed * 0.5f * Time.fixedDeltaTime);
            next.y += Mathf.Sin(Time.time * bobSpeed) * 0.02f;
            rb.MovePosition(next);
        }
    }
    private bool CheckLineOfSight()
    {
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        return Physics2D.Raycast(transform.position, dir, dist, obstacleLayer).collider == null;
    }

    public void TakeDamage(float damage)
    {
        if (!isFightStarted) StartBossFight();

        if (!isStunned && !isCharging)
        {
            currentPoise -= damage * 2f;
            if (bossHUD != null) bossHUD.UpdatePoise(currentPoise);

            if (currentPoise <= 0)
            {
                StartCoroutine(StunState());
            }
        }
    }

    IEnumerator StunState()
    {
        isStunned = true;
        animator.SetTrigger("Hurt");

        Color old = mySprite.color;
        mySprite.color = Color.gray; 

        yield return new WaitForSeconds(2.0f); 

        currentPoise = maxPoise; 
        if (bossHUD != null) bossHUD.UpdatePoise(currentPoise);

        mySprite.color = old;
        isStunned = false;
    }

    void DecideAttack()
    {
        int chance = isRaging ? teleportChance + 20 : teleportChance;

        if (Random.Range(0, 100) < chance)
        {
            StartCoroutine(TeleportSkill());
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }
    
    IEnumerator TeleportSkill()
    {
        isTeleporting = true;
        animator.SetBool("isMoving", false);

        if (mySprite) mySprite.enabled = false;
        if (myCollider) myCollider.enabled = false;

        yield return new WaitForSeconds(teleportDuration);

        float dir = -Mathf.Sign(playerTransform.localScale.x);
        Vector2 teleportPos = new Vector2(
            playerTransform.position.x + dir * teleportOffset,
            playerTransform.position.y + flyHeight
        );
        transform.position = teleportPos;

        if (mySprite) mySprite.enabled = true;
        if (myCollider) myCollider.enabled = true;

        float newFacing = (playerTransform.position.x > transform.position.x) ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * newFacing, transform.localScale.y, 1);

        float timer = 0;
        float wait = isRaging ? postTeleportIdleTime * 0.5f : postTeleportIdleTime;
        Color baseColor = isRaging ? rageColor : Color.white;

        while (timer < wait)
        {
            newFacing = (playerTransform.position.x > transform.position.x) ? 1 : -1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * newFacing, transform.localScale.y, 1);

            if (mySprite)
                mySprite.color = Color.Lerp(baseColor, Color.yellow, Mathf.PingPong(Time.time * 20, 1));

            timer += Time.deltaTime;
            yield return null;
        }

        if (mySprite) mySprite.color = baseColor;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        isTeleporting = false;
    }
    
    public void ActivateRage()
    {
        if (isRaging) return;
        StartCoroutine(RageTransitionSequence());
    }

    IEnumerator RageTransitionSequence()
    {
        isRaging = true;
        isCharging = true; 

        if (myCollider) myCollider.enabled = false;

        float elapsed = 0f;
        float nextRockTime = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsed < chargeDuration)
        {
            transform.position = startPos + (Vector3)Random.insideUnitCircle * 0.1f;
            if (mySprite) mySprite.color = Color.Lerp(Color.white, rageColor, Mathf.PingPong(Time.time * 20, 1));

            if (Time.time >= nextRockTime)
            {
                SpawnFallingRock();
                nextRockTime = Time.time + rockSpawnRate;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
        if (myCollider) myCollider.enabled = true;
        isCharging = false;

        moveSpeed *= 1.4f;
        attackCooldown *= 0.6f;
        spawnDelay /= 1.5f;
        teleportDuration *= 0.7f;

        if (mySprite) mySprite.color = rageColor;

        transform.localScale = new Vector3(
            Mathf.Sign(transform.localScale.x) * Mathf.Abs(originalScale.x) * 1.3f,
            originalScale.y * 1.3f, 1);

        if (rageVFX != null) Instantiate(rageVFX, transform.position, Quaternion.identity);
    }

    void SpawnFallingRock()
    {
        if (rockPrefab == null) return;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        Vector3 camPos = Camera.main.transform.position;
        float randomX = Random.Range(camPos.x - camWidth, camPos.x + camWidth);
        float groundY = playerTransform.position.y - 1.5f;

        Instantiate(rockPrefab, new Vector3(randomX, groundY, 0), Quaternion.identity);
    }
    
    public void ShootProjectile()
    {
        if (Random.Range(0, 100) < (isRaging ? 70 : 30))
            StartCoroutine(ShootSpread());
        else
            StartCoroutine(SpawnChainAttack(0));
    }

    IEnumerator SpawnChainAttack(float angleOffset)
    {
        if (firePoint == null) yield break;

        Vector2 dir = (playerTransform.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Vector3 finalDir = rot * Vector3.right;

        if (part1Prefab) Instantiate(part1Prefab, firePoint.position, rot);
        yield return new WaitForSeconds(spawnDelay);

        if (part2Prefab) Instantiate(part2Prefab, firePoint.position + finalDir * segmentLength, rot);
        yield return new WaitForSeconds(spawnDelay);

        if (part3Prefab) Instantiate(part3Prefab, firePoint.position + finalDir * segmentLength * 2, rot);
    }

    IEnumerator ShootSpread()
    {
        StartCoroutine(SpawnChainAttack(0));
        StartCoroutine(SpawnChainAttack(25));
        StartCoroutine(SpawnChainAttack(-25));
        yield return null;
    }
}