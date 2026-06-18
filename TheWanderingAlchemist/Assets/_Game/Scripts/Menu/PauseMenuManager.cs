using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel; 
    [SerializeField] private GameObject controlsPanel; 

    private bool isPaused = false;
    private bool wasOpenedThisFrame = false;

    private void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (controlsPanel != null && controlsPanel.activeSelf)
            {
                CloseControlsButton();
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (controlsPanel != null && controlsPanel.activeSelf && !wasOpenedThisFrame)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                        controlsPanel.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    CloseControlsButton();
                }
            }
        }

        if (wasOpenedThisFrame && Input.GetMouseButtonUp(0))
        {
            wasOpenedThisFrame = false;
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
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
        
        AudioListener.pause = false;
    }

    public void OpenControlsButton()
    {
        if (controlsPanel != null)
        {
            wasOpenedThisFrame = true; 
            
            controlsPanel.SetActive(true);
            controlsPanel.transform.SetAsLastSibling();
        }
    }

    public void CloseControlsButton()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
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

        if (PlayerStats.Instance != null) 
        {
            Destroy(PlayerStats.Instance.gameObject);
        }

        GameObject bigManager = GameObject.Find("Manager");
        if (bigManager != null)
        {
            Destroy(bigManager);
        }

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene("Menu"); 
        }
    }
}