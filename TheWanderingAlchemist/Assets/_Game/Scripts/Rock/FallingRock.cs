using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public int damage = 15;
    public GameObject impactVFX;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        float randomTorque = Random.Range(-300f, 300f);
        rb.angularVelocity = randomTorque;
        Destroy(gameObject, 5f); 
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player") || hitInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (hitInfo.CompareTag("Player"))
            {
                var player = hitInfo.GetComponent<PlayerStats>(); 
                if (player != null) player.TakeDamage(damage);
            }

            if (impactVFX != null)
            {
                Instantiate(impactVFX, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}