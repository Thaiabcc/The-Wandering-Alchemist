using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" || scene.name == "Ending")
        {
            ShowManager(false);
        }
        else
        {
            Invoke(nameof(ForceShowHUD), 0.1f);
        }
    }

    private void ForceShowHUD()
    {
        ShowManager(true);
    }

    public void ShowManager(bool show)
    {
        HUDComponent[] foundHuds = Resources.FindObjectsOfTypeAll<HUDComponent>();
        if (foundHuds.Length > 0) 
        {
            foreach (var hud in foundHuds)
            {
                if (hud.gameObject.scene.isLoaded)
                {
                    hud.gameObject.SetActive(show);
                }
            }
        }
    }
}