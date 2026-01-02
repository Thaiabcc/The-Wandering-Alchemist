using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Movement & Range")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;

    [Header("Patrol Settings")]
    public float patrolRadius = 3f;
    public float waitTime = 2f;
    public float stoppingDistance = 0.2f;

    [Header("Environment")]
    public LayerMask obstacleLayer;
    public float combatCenterOffset = 0.5f;
}
