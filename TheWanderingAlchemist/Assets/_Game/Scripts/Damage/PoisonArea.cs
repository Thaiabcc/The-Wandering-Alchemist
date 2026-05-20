using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    [Header("Poison Settings")]
    public float poisonDamage = 1f;      
    public float damageInterval = 2f; 

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.ApplyPoison(poisonDamage, damageInterval);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.CurePoison();
            }
        }
    }
}