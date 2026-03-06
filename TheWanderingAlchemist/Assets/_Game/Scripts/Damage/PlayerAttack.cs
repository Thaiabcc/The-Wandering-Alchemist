using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Stats")]
    [SerializeField] private float critChance = 30f;
    [SerializeField] private float critMultiplier = 2f;
    [SerializeField] private float staminaCost = 10f;

    [Header("Timing & Settings")]
    [SerializeField] private float hitDelay = 0.3f;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private float spawnDistance = 1.0f;
    [SerializeField] private float heightOffset = 0.8f;

    private bool isAttacking;
    private Vector2 mouseDirection;
    private float lastAttackTime;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        CalculateMouseDirection();
        UpdateAttackPointTransform();
        if (Input.GetButton("Fire1") && Time.time >= lastAttackTime + cooldown && !isAttacking)
        {
            if (PlayerStats.Instance != null && PlayerStats.Instance.TryConsumeStamina(staminaCost))
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                // Hết mana/stamina
                // Debug.Log("Cạch cạch!"); 
            }
        }
    }

    private void CalculateMouseDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 playerCenter = transform.position + Vector3.up * heightOffset;
        mouseDirection = (mousePos - playerCenter).normalized;
    }

    private void UpdateAttackPointTransform()
    {
        if (attackPoint)
        {
            Vector3 playerCenter = transform.position + Vector3.up * heightOffset;
            attackPoint.position = playerCenter + (Vector3)(mouseDirection * spawnDistance);

            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            attackPoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    private void FaceMouseDirection()
    {
        /*if (spriteRenderer != null)
        {
            // Nếu chuột bên trái -> Flip, ngược lại thì thôi
            spriteRenderer.flipX = (mouseDirection.x < 0);
        }*/

        // Cập nhật Animator để phát animation đánh đúng hướng (nếu game có anim 4 hướng)
        if (animator != null)
        {
            animator.SetFloat("Horizontal", mouseDirection.x);
            animator.SetFloat("Vertical", mouseDirection.y);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Khi bắt đầu đánh, ta ép nhân vật quay mặt về hướng chuột 1 cái
        // Để tránh việc bắn ra sau lưng
        FaceMouseDirection();
        // ----------------------

        animator.SetTrigger("Attack");
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.swordSwing, 0.5f);

        // Chờ đến thời điểm vung tay/bắn ra
        yield return new WaitForSeconds(hitDelay);

        // Bắn đạn
        SpawnProjectile();

        // (Tùy chọn) Nếu muốn trong lúc đánh nhân vật vẫn luôn nhìn theo chuột thì bỏ FaceMouseDirection() vào Update với điều kiện if(isAttacking)

        isAttacking = false;
    }

    private void SpawnProjectile()
    {
        if (rockPrefab == null) return;

        // Cập nhật lại hướng bắn 1 lần nữa ngay lúc đạn bay ra cho chuẩn (nếu người chơi vẩy chuột nhanh)
        CalculateMouseDirection();

        float statsDamage = (PlayerStats.Instance != null) ? PlayerStats.Instance.currentDamage : 10;
        bool isCrit = Random.Range(0f, 100f) < critChance;
        int finalDamage = Mathf.RoundToInt(isCrit ? statsDamage * critMultiplier : statsDamage);

        GameObject rock = Instantiate(rockPrefab, attackPoint.position, Quaternion.identity);
        RockProjectile rockScript = rock.GetComponent<RockProjectile>();

        if (rockScript != null)
        {
            rockScript.Setup(mouseDirection, finalDamage, isCrit);
        }
    }
}