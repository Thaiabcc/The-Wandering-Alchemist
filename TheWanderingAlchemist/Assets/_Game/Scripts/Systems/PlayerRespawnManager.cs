using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager Instance { get; private set; }

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
        yield return new WaitForSeconds(1.6f);

        // Teleport & Reset player
        Vector3 spawnPosition = Vector3.zero;
        if (defaultSpawnPoint != null)
        {
            spawnPosition = defaultSpawnPoint.position;
        }
        else
        {
            GameObject spawnObj = GameObject.Find("PlayerSpawnPoint");
            if (spawnObj != null) spawnPosition = spawnObj.transform.position;
        }

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.transform.position = spawnPosition;
            Physics2D.SyncTransforms();
            PlayerStats.Instance.HealFullAndReset();
        }

        PlayerPenalty penalty = FindObjectOfType<PlayerPenalty>();
        if (penalty != null) penalty.ApplyPenalty();

        if (HotbarManager.Instance != null)
            HotbarManager.Instance.UpdateAllSlotsUI();

        // Ẩn chữ "BẠN ĐÃ CHẾT"
        if (DeathUI.Instance != null && DeathUI.Instance.deadText != null)
            DeathUI.Instance.deadText.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.4f);

        string currentScene = SceneManager.GetActiveScene().name;

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchSceneFromDeath(currentScene);
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }

        yield return new WaitForSeconds(0.5f);

        if (DeathUI.Instance != null)
            DeathUI.Instance.ResetUI();
    }
}