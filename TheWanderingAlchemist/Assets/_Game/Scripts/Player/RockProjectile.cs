using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;

    // [MỚI] Tầm xa tối đa. Bay hết tầm này là tự vỡ.
    public float maxRange = 6f;

    [Header("Visual Effects")]
    public float fallGravity = 1.5f;
    public float destroyAfterHit = 0.4f;

    private int damage;
    private bool isCrit;
    private bool hasHit = false;
    private Vector2 startPosition; // [MỚI] Để lưu vị trí lúc mới bắn

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        // [MỚI] Lưu lại vị trí xuất phát
        startPosition = transform.position;

        // [XÓA] Dòng này cũ rồi, xóa đi vì giờ mình quản lý việc chết bằng Range và BreakAndFall
        // Destroy(gameObject, lifeTime); 
    }

    public void Setup(Vector2 dir, int dmg, bool crit)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        damage = dmg;
        isCrit = crit;
    }

    private void Update()
    {
        if (hasHit) return;

        // 1. Di chuyển viên đạn
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 2. [MỚI] Kiểm tra khoảng cách đã bay
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange)
        {
            // Nếu bay quá xa -> Tự vỡ
            BreakAndFall();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Monster"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                DamagePopupGenerator.Instance?.Create(transform.position, damage, isCrit);

                if (isCrit)
                {
                    CameraShake.Instance?.Shake(0.15f, 2f);
                    HitStop.Instance?.Stop(0.05f);
                }

                BreakAndFall();
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            BreakAndFall();
        }
    }

    private void BreakAndFall()
    {
        if (hasHit) return; // Kiểm tra lại lần nữa cho chắc
        hasHit = true;

        // 1. Tắt va chạm
        if (col != null) col.enabled = false;

        // 2. Chạy Animation vỡ
        if (anim != null) anim.Play("Rock_Break");

        // Audio
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.stoneBreak, 0.8f, true);
        // 3. Hiệu ứng rơi
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Dừng lại ngay tại chỗ hết tầm
            rb.gravityScale = fallGravity;
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-100f, 100f));
        }

        // 4. Hủy object sau khi animation chạy xong (khoảng 0.4s)
        Destroy(gameObject, destroyAfterHit);
    }
}