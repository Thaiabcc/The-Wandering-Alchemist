using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float maxRange = 6f;

    [Header("Visual Effects")]
    public float fallGravity = 1.5f;
    public float destroyAfterHit = 0.4f;

    private int damage;
    private bool isCrit;
    private bool hasHit = false;
    private Vector2 startPosition;

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
        startPosition = transform.position;
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
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange)
        {
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
                 collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            BreakAndFall();
        }
    }

    private void BreakAndFall()
    {
        if (hasHit) return; 
        hasHit = true;
        if (col != null) col.enabled = false;
        if (anim != null) anim.Play("Rock_Break");
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.stoneBreak, 0.8f, true);
        if (rb != null)
        {
            rb.velocity = Vector2.zero; 
            rb.gravityScale = fallGravity;
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-100f, 100f));
        }
        Destroy(gameObject, destroyAfterHit);
    }
}