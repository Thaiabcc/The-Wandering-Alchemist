using UnityEngine;
using System.Collections;

public class BossChainAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 15f;
    public bool applyPoison = false;
    public float poisonDamage = 2f;
    public float poisonInterval = 1f;

    [Header("Life Time")]
    public float lifeTime = 0.5f;
    
    [Header("Visual Effects")]
    public GameObject hitEffectPrefab;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.TakeDamage(damage);

                if (applyPoison)
                {
                    PlayerStats.Instance.ApplyPoison(poisonDamage, poisonInterval);
                }

                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                }
            }

            // Destroy(gameObject);
        }
    }
}