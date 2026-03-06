using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Respawn Settings")]
    public Transform fixedSpawnPoint;
    public GameObject player;
    public bool applyPenalty = true;

    [Header("Teleport Data")]
    public Vector3? nextSpawnPosition;

    [Header("Return To World")]
    public string lastWorldScene;
    public Vector3 lastWorldPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RespawnPlayer()
    {
        if (player == null || fixedSpawnPoint == null)
        {
            Debug.LogWarning("GameManager: Missing player or spawn point");
            return;
        }

        // Teleport position
        player.transform.position = fixedSpawnPoint.position;

        // Reset stats
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.HealFullAndReset();
        }

        // Apply penalty
        if (applyPenalty)
        {
            var penalty = player.GetComponent<PlayerPenalty>();
            if (penalty != null)
            {
                penalty.ApplyPenalty();
            }
        }
    }
}
