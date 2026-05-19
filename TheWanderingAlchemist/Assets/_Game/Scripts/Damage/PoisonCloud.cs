using UnityEngine;
using System.Collections.Generic;

public class PoisonCloud : MonoBehaviour
{
    [Header("Poison Cloud Stats")]
    public float damagePerTick;
    public float tickInterval = 0.5f; 
    public float cloudDuration = 5f; 

    private float tickTimer;
    private List<Collider2D> enemiesInCloud = new List<Collider2D>();

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            ApplyPoisonDamage();
        }
    }

    public void InitCloud(float calculatedDamage)
    {
        damagePerTick = calculatedDamage;
        Destroy(gameObject, cloudDuration);
    }

    private void ApplyPoisonDamage()
    {
        for (int i = enemiesInCloud.Count - 1; i >= 0; i--)
        {
            Collider2D enemyCollider = enemiesInCloud[i];

            if (enemyCollider == null || !enemyCollider.gameObject.activeInHierarchy)
            {
                enemiesInCloud.RemoveAt(i);
                continue;
            }

            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerTick, false, true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") && !enemiesInCloud.Contains(collision))
        {
            enemiesInCloud.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") && enemiesInCloud.Contains(collision))
        {
            enemiesInCloud.Remove(collision);
        }
    }
}