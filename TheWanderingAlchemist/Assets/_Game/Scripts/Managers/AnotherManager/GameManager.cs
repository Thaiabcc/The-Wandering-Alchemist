using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Respawn Settings")]
    public Transform fixedSpawnPoint;
    public GameObject player;
    public bool applyPenalty = true;

    private static Vector3? _nextSpawnPosition;
    public Vector3? nextSpawnPosition
    {
        get { return _nextSpawnPosition; }
        set
        {
            _nextSpawnPosition = value;
        }
    }

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
    }

    public void RespawnPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null || fixedSpawnPoint == null)
        {
            Debug.LogWarning("GameManager: Missing player or spawn point");
            return;
        }

        player.transform.position = fixedSpawnPoint.position;

        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.HealFullAndReset();
        }

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