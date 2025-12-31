using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Teleport Data")]
    public Vector3? nextSpawnPosition;

    [Header("Return To World")]
    public string lastWorldScene;
    public Vector3 lastWorldPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
