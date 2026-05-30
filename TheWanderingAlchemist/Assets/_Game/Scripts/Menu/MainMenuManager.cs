using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] public Button continueButton;
    
    [Header("Scene Settings")]
    public string firstSceneName = "Town 1";

    [Header("Audio")]
    public AudioMixer mainMixer;

    [Header("UI & Settings")]
    public GameObject settingsPanel;
    public CanvasGroup settingsCanvasGroup;
    
    [Header("Credits UI")]
    public GameObject creditsPanel;
    public CanvasGroup creditsCanvasGroup;
    public RectTransform creditsTextRect;
    public float creditsScrollSpeed = 100f;

    [Header("Cinematic Intro (New Game)")]
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup loreCanvasGroup;
    public TMP_Text loreText;

    public float timeToDrive = 3f;
    public float typingSpeed = 0.05f;
    public float timeToReadLore = 5f;
    
    public UnityEvent onWagonStartDriving;

    [Header("Effect Speed")]
    public float fadeSpeed = 5f;

    private Vector2 originalCreditsPos;
    private Coroutine creditsRoutine;
    private bool isStartingGame = false;

    public void Start()
    {
        bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSaveFile();
        continueButton.interactable = hasSave;
        Color cl = continueButton.image.color;
        cl.a = hasSave ? 1f : 0.5f;
        continueButton.image.color = cl;
        
        if (creditsTextRect != null)
        {
            originalCreditsPos = creditsTextRect.anchoredPosition;
        }

        if (loreCanvasGroup != null)
        {
            loreCanvasGroup.alpha = 0f;
            loreCanvasGroup.blocksRaycasts = false;
            loreCanvasGroup.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (isStartingGame) return;
        isStartingGame = true;

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;
        }

        StartCoroutine(NewGameCinematicSequence());
    }

    IEnumerator NewGameCinematicSequence()
    {
        if (mainMenuCanvasGroup != null)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * (fadeSpeed / 2f);
                mainMenuCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }

        onWagonStartDriving?.Invoke();
        yield return new WaitForSeconds(timeToDrive);

        if (loreCanvasGroup != null)
        {
            loreCanvasGroup.gameObject.SetActive(true);
            loreCanvasGroup.alpha = 0f;

            if (loreText != null)
            {
                loreText.maxVisibleCharacters = 0;
            }

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * (fadeSpeed / 2f);
                loreCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            if (loreText != null)
            {
                loreText.ForceMeshUpdate();
                int totalCharacters = loreText.textInfo.characterCount;
                int visibleCount = 0;
                while (visibleCount <= totalCharacters)
                {
                    loreText.maxVisibleCharacters = visibleCount;
                    visibleCount++;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            yield return new WaitForSeconds(timeToReadLore);
        }

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(firstSceneName, () => 
            {
                Time.timeScale = 1f;
            });
        }
        else
        {
            SceneManager.LoadScene(firstSceneName);
        }
    }

    public void ContinueGame()
    {
        if (isStartingGame) return;
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveFile())
        {
            string sceneToLoad = SaveManager.Instance.GetSavedSceneName();
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.SwitchScene(sceneToLoad, () => 
                {
                    SaveManager.Instance.LoadGame();
                    Time.timeScale = 1f;
                });
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
                SaveManager.Instance.LoadGame();
                Time.timeScale = 1f;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        StartCoroutine(FadePanel(0f, 1f, 0.9f, 1f));
    }

    public void CloseSettings()
    {
        StartCoroutine(FadePanel(1f, 0f, 1f, 0.9f, true));
    }
    
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        if (creditsRoutine != null) StopCoroutine(creditsRoutine);
        creditsRoutine = StartCoroutine(ScrollCreditsSequence());
    }

    public void CloseCredits()
    {
        if (creditsRoutine != null) StopCoroutine(creditsRoutine);
        StartCoroutine(FadeOutCredits());
    }

    IEnumerator ScrollCreditsSequence()
    {
        creditsTextRect.anchoredPosition = new Vector2(originalCreditsPos.x, -(creditsTextRect.rect.height + 1080f)); 
        
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            creditsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time);
            yield return null;
        }

        while (true)
        {
            creditsTextRect.anchoredPosition += Vector2.up * creditsScrollSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOutCredits()
    {
        float time = 0;
        float startAlpha = creditsCanvasGroup.alpha;
        
        while (time < 0.5f) 
        {
            time += Time.deltaTime * 2f;
            creditsCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time);
            yield return null;
        }
        creditsPanel.SetActive(false);
    }

    IEnumerator FadePanel(float startAlpha, float endAlpha, float startScale, float endScale, bool disableAfter = false)
    {
        float time = 0;
        settingsCanvasGroup.alpha = startAlpha;
        settingsPanel.transform.localScale = new Vector3(startScale, startScale, 1f);

        while (time < 1f)
        {
            time += Time.deltaTime * fadeSpeed;
            settingsCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time);
            float currentScale = Mathf.Lerp(startScale, endScale, time);
            settingsPanel.transform.localScale = new Vector3(currentScale, currentScale, 1f);
            yield return null;
        }

        if (disableAfter)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("BGMVol", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }
}