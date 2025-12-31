using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // [MỚI 1] Thêm thư viện này để check UI

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;

    [Header("Combat Stats")]
    [SerializeField] private float critChance = 30f;
    [SerializeField] private float critMultiplier = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Timing")]
    [SerializeField] private float hitDelay = 0.1f;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private float attackOffset = 0.8f;

    private bool isAttacking;
    private Vector2 attackDirection = Vector2.right;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // [MỚI 2] Kiểm tra nếu chuột đang đè lên UI (Nút, Bảng, Thanh máu...)
        // IsPointerOverGameObject() trả về true nếu chuột đang trỏ vào bất kỳ object UI nào có Raycast Target
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // Dừng lại ngay, không chạy logic bên dưới
        }
        // -----------------------------------------------------------

        UpdateAttackDirection();
        UpdateAttackPoint();

        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // ------------------ DIRECTION ------------------

    private void UpdateAttackDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0) attackDirection = x > 0 ? Vector2.right : Vector2.left;
        else if (y != 0) attackDirection = y > 0 ? Vector2.up : Vector2.down;
    }

    private void UpdateAttackPoint()
    {
        if (attackPoint)
            attackPoint.localPosition = attackDirection * attackOffset;
    }

    // ------------------ ATTACK ------------------

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        PlayAttackAnimation();
        PlaySwingSound();

        yield return new WaitForSeconds(hitDelay);

        DealDamage();

        yield return new WaitForSeconds(Mathf.Max(0, cooldown - hitDelay));
        isAttacking = false;
    }

    private void DealDamage()
    {
        // 1. Quét va chạm
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        if (hits.Length == 0) return;

        // 2. Âm thanh
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.enemyHit, 0.5f);

        // 3. Hashset chém quái nhiều lần
        HashSet<EnemyHealth> damaged = new HashSet<EnemyHealth>();

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null || damaged.Contains(enemy)) continue;

            // ===========================================================
            // LOGIC TÍNH DAME
            // ===========================================================

            // Lấy dame từ PlayerStats
            float statsDamage = 0;
            if (PlayerStats.Instance != null)
            {
                statsDamage = PlayerStats.Instance.currentDamage;
            }
            else
            {
                // Fallback: Nếu quên chưa bỏ PlayerStats vào scene thì lấy tạm số 10
                statsDamage = 10;
                Debug.LogWarning("Thiếu PlayerStats trong Scene!");
            }

            // Chí mạng
            bool isCrit = Random.Range(0f, 100f) < critChance;

            // Final Damage 
            float calculatedDamage = isCrit
                ? statsDamage * critMultiplier
                : statsDamage;

            // Làm tròn Damage 
            int finalDamage = Mathf.RoundToInt(calculatedDamage);

            // ===========================================================
            enemy.TakeDamage(finalDamage);
            damaged.Add(enemy);
            DamagePopupGenerator.Instance?.Create(
                hit.transform.position,
                finalDamage,
                isCrit
            );
        }
    }

    private void PlayAttackAnimation()
    {
        if (!animator) return;

        animator.SetFloat("InputX", attackDirection.x);
        animator.SetFloat("InputY", attackDirection.y);
        animator.SetTrigger("Attack");
    }

    private void PlaySwingSound()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.swordSwing, 0.5f);
    }

    // ------------------ DEBUG ------------------

    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}