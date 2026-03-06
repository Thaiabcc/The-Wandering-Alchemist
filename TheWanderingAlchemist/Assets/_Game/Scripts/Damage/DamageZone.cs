using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Sức mạnh")]
    [SerializeField] private int damage = 1;
    [SerializeField] private string targetTag = "Monster"; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }
}
