using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GlobalInputClick : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickClip;

    [Header("Visual Effect Settings")]
    [SerializeField] private GameObject ripplePrefab;
    [SerializeField] private string menuSceneName = "Menu";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckScriptStatus();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScriptStatus();
    }

    private void CheckScriptStatus()
    {
        enabled = SceneManager.GetActiveScene().name == menuSceneName;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                PlaySoundOnly();
            }
            else
            {
                PlaySoundAndEffect();
            }
        }
    }

    private void PlaySoundOnly()
    {
        if (audioSource != null && clickClip != null)
        {
            float master = PlayerPrefs.GetFloat("MasterVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
            audioSource.PlayOneShot(clickClip, 1f * sfx * master);
        }
    }

    private void PlaySoundAndEffect()
    {
        PlaySoundOnly();

        if (ripplePrefab != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Instantiate(ripplePrefab, mousePos, Quaternion.identity);
        }
    }
}