using UnityEngine;

public class PoisonPotion : MonoBehaviour
{
    [Header("Movement & Arc")]
    public float speed = 12f;
    public float lifetime = 0.8f;
    public float rotationSpeed = 720f;
    public float maxVisualHeight = 1.5f; 
    public Transform visualChild;       

    [Header("Poison Settings")]
    public GameObject poisonCloudPrefab;

    private Vector2 moveDirection;
    private bool hasImpacted = false;
    private float lifeTimer = 0f;
    private float finalDamage;

    public void Setup(Vector2 direction, float damage)
    {
        moveDirection = direction.normalized;
        finalDamage = damage;
    }

    private void Update()
    {
        if (hasImpacted) return;

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
            DeployPoison();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasImpacted) return;
        if (lifeTimer < 0.08f) return;

        if (collision.CompareTag("Monster") || collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            DeployPoison();
        }
    }

    private void DeployPoison()
    {
        hasImpacted = true;

        if (poisonCloudPrefab != null)
        {
            GameObject cloud = Instantiate(poisonCloudPrefab, transform.position, Quaternion.identity);
            cloud.GetComponent<PoisonCloud>()?.InitCloud(finalDamage);
        }

        if (AudioManager.Instance != null && AudioManager.Instance.potionUse != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.potionUse, 1f, true);
        }

        Destroy(gameObject);
    }
}