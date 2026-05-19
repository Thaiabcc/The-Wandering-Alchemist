using UnityEngine;

public class ExplosivePotion : MonoBehaviour
{
    [Header("Movement & Arc")]
    public float speed = 12f;
    public float lifetime = 0.8f;     
    public float rotationSpeed = 720f;  
    public float maxVisualHeight = 1.5f; 
    public Transform visualChild;       

    [Header("Explosion Settings")]
    public float explosionRadius = 2.5f;
    public float explosionDamage = 50f;
    public GameObject explosionVFXPrefab; 
    public LayerMask enemyLayer;           

    private Vector2 moveDirection;
    private bool hasExploded = false;
    private float lifeTimer = 0f;       

    public void Setup(Vector2 direction)
    {
        moveDirection = direction.normalized;
        lifeTimer = 0f;
    }

    private void Update()
    {
        if (hasExploded) return;

        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);

        lifeTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(lifeTimer / lifetime);
        float height = 4f * maxVisualHeight * progress * (1f - progress);

        if (visualChild != null)
        {
            visualChild.localPosition = new Vector3(0, height, 0);
            visualChild.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if (lifeTimer >= lifetime)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;
        if (lifeTimer < 0.08f) return;
        if (collision.isTrigger) return;
        if (collision.CompareTag("Monster") || collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;
        Vector3 explosionPos = visualChild != null ? visualChild.position : transform.position;
        if (explosionVFXPrefab != null)
        {
            Instantiate(explosionVFXPrefab, explosionPos, Quaternion.identity);
        }

        if (AudioManager.Instance != null && AudioManager.Instance.explosionSFX != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.explosionSFX, 1f, true);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage, false, false);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}