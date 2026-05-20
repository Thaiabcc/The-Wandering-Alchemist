using UnityEngine;
using System.Collections;

public class BossAI : EnemyAI
{
    [Header("--- CHIẾN ĐẤU RIÊNG CỦA BOSS ---")]
    public int damageAmount = 10;
    public float hitRange = 1.0f;
    public Transform attackPoint;

    [Header("--- HIỆU ỨNG (PREFABS) ---")]
    public GameObject vfxAttack1;
    public GameObject vfxAttack2;
    [Tooltip("Thời gian tồn tại của VFX")]
    [SerializeField] private float vfxDuration = 0.4f; 

    private bool canAttack = true;
    private int currentAttackType = 1;

    protected override void Start()
    {
        base.Start();
        // chaseRange = 999f;
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;
        if (playerTransform == null) return;
        if (playerTransform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > attackRange)
        {
            if (canAttack)
            {
                animator.SetBool("isMoving", true);
                animator.SetFloat("Speed", 1);

                Vector2 targetPos = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(targetPos);
            }
        }
        else
        {
            StopMoving();
            animator.SetFloat("Speed", 0);

            if (canAttack)
            {
                StartCoroutine(BossAttackCombo());
            }
        }
    }

    IEnumerator BossAttackCombo()
    {
        canAttack = false;
        int rand = Random.Range(1, 3);
        currentAttackType = rand;

        animator.SetInteger("AttackIndex", currentAttackType);
        animator.SetTrigger("AttackTrigger");

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // --- HÀM VFX ---
    public void SpawnSkillEffect()
    {
        GameObject vfxToSpawn = (currentAttackType == 1) ? vfxAttack1 : vfxAttack2;

        if (vfxToSpawn != null && attackPoint != null)
        {
            GameObject vfx = Instantiate(vfxToSpawn, attackPoint.position, Quaternion.identity);

            // --- LOGIC FIX HƯỚNG ---
            Vector3 newScale = vfx.transform.localScale;
            newScale.x = Mathf.Abs(newScale.x);

            if (transform.localScale.x < 0)
            {
                newScale.x = -newScale.x;
            }

            vfx.transform.localScale = newScale;
            Destroy(vfx, vfxDuration);
        }

        // --- XỬ LÝ DAME ---
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, hitRange);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                var healthScript = obj.GetComponent<PlayerStats>();
                if (healthScript != null) healthScript.TakeDamage(damageAmount);
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, hitRange);
        }
    }
}