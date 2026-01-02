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

    // 👇 [MỚI] Thêm tốn thể lực khi đánh
    [Header("Stamina Cost")]
    [Tooltip("Mỗi lần bắn tốn bao nhiêu thể lực")]
    [SerializeField] private float staminaCost = 10f;

    [Header("Timing & Settings")]
    [Tooltip("Delay để khớp animation vung tay")]
    [SerializeField] private float hitDelay = 0.3f;
    [SerializeField] private float cooldown = 0.5f;

    [Tooltip("Khoảng cách điểm bắn so với người")]
    [SerializeField] private float spawnDistance = 1.0f;
    [Tooltip("Độ cao điểm bắn (để bắn từ ngực/đầu thay vì chân)")]
    [SerializeField] private float heightOffset = 0.8f;

    private bool isAttacking;
    private Vector2 mouseDirection;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        UpdateMouseDirection();

        if (isAttacking) return;

        if (Input.GetButtonDown("Fire1"))
        {
            // 👇 [MỚI] Kiểm tra Stamina trước khi đánh
            // Hàm TryConsumeStamina sẽ tự trừ và trả về true nếu đủ, false nếu thiếu
            if (PlayerStats.Instance != null && PlayerStats.Instance.TryConsumeStamina(staminaCost))
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                // (Tùy chọn) Hiệu ứng khi hết hơi (vd: Âm thanh "cạch cạch")
                Debug.Log("Hết hơi rồi, không bắn được!");
            }
        }
    }

    private void UpdateMouseDirection()
    {
        // 1. Lấy vị trí chuột
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 2. Tính tâm nhân vật
        Vector3 playerCenter = transform.position + Vector3.up * heightOffset;

        // 3. Tính hướng
        mouseDirection = (mousePos - playerCenter).normalized;

        // 4. Cập nhật vị trí AttackPoint
        if (attackPoint)
        {
            attackPoint.position = playerCenter + (Vector3)(mouseDirection * spawnDistance);

            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            attackPoint.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 5. Lật Sprite theo hướng chuột
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (mouseDirection.x < 0);
        }

        // 6. Cập nhật Animator
        if (!isAttacking && animator != null)
        {
            animator.SetFloat("InputX", mouseDirection.x);
            animator.SetFloat("InputY", mouseDirection.y);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        animator.SetTrigger("Attack");
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.swordSwing, 0.5f);

        yield return new WaitForSeconds(hitDelay);

        SpawnProjectile();

        yield return new WaitForSeconds(Mathf.Max(0, cooldown - hitDelay));
        isAttacking = false;
    }

    private void SpawnProjectile()
    {
        if (rockPrefab == null) return;

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