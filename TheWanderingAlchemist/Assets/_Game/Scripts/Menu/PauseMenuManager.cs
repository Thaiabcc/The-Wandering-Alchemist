using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel; 

    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
        
        AudioListener.pause = true; 
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
        
        AudioListener.pause = false;
    }

    public void SaveGameButton()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("Save Data Success");
        }
        else
        {
            Debug.LogError("Not found save manager");
        }
    }

    public void BackToMenuButton()
    {
        Time.timeScale = 1f; 
        isPaused = false;
        AudioListener.pause = false; 

        if (PlayerStats.Instance != null) Destroy(PlayerStats.Instance.gameObject);
        Destroy(GameObject.Find("HotbarManager"));
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("QuestManager"));
        Destroy(GameObject.Find("InventoryManager"));
        Destroy(GameObject.Find("UI_Manager"));
        Destroy(GameObject.Find("HUD_Player"));
        Destroy(GameObject.Find("Respawn Manager"));

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene("Menu"); 
        }
    }
}