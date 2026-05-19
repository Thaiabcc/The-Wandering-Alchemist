using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager Instance { get; private set; }

    [Header("Điểm Hồi Sinh Mặc Định")]
    [Tooltip("Kéo Empty Object làm điểm spawn vào đây")]
    [SerializeField] private Transform defaultSpawnPoint;

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
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(2.0f);

        Vector3 spawnPosition;

        if (defaultSpawnPoint != null)
        {
            spawnPosition = defaultSpawnPoint.position;
        }
        else
        {
            GameObject spawnObj = GameObject.Find("PlayerSpawnPoint");
            spawnPosition = spawnObj != null ? spawnObj.transform.position : Vector3.zero;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.SwitchScene(currentScene);
        else
            SceneManager.LoadScene(currentScene);

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == currentScene);

        yield return new WaitForSeconds(0.5f);

        // Reset Player
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.transform.position = spawnPosition;
            PlayerStats.Instance.HealFullAndReset();
        }

        PlayerPenalty penalty = FindObjectOfType<PlayerPenalty>();
        if (penalty != null) 
            penalty.ApplyPenalty();

        // Refresh Hotbar sau khi respawn
        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.UpdateAllSlotsUI();
        }

        // Fade UI
        if (DeathUI.Instance != null)
        {
            yield return StartCoroutine(DeathUI.Instance.FadeOutBlack(1.5f));
        }
    }
}