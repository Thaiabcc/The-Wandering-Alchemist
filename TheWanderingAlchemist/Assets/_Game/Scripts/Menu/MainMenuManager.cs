using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] public Button continueButton;

    [Header("Scene Settings")]
    public string firstSceneName = "Town 1";

    [Header("UI & Settings")]
    public GameObject settingsPanel;
    public CanvasGroup settingsCanvasGroup;

    [Header("Credits UI")]
    public GameObject creditsPanel;
    public CanvasGroup creditsCanvasGroup;
    public RectTransform creditsTextRect;
    public float creditsStartY = -800f;
    public float creditsScrollSpeed = 100f;

    [Header("Intro (New Game)")]
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup loreCanvasGroup;
    public TMP_Text loreText;
    public float timeToDrive = 3f;
    public float typingSpeed = 0.05f;
    public float timeToReadLore = 5f;
    public UnityEvent onWagonStartDriving;

    [Header("Effect Speed")]
    public float fadeSpeed = 5f;
    
    [Header("Volume Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private Vector2 originalCreditsPos;
    private Coroutine creditsRoutine;
    private bool isStartingGame = false;

    public void Start()
    {
        bool hasSave = SaveManager.Instance != null && SaveManager.Instance.IsSaveValid();
        if (continueButton != null)
        {
            continueButton.interactable = hasSave;
            CanvasGroup group = continueButton.GetComponent<CanvasGroup>();
            if (group != null) group.alpha = hasSave ? 1f : 0.5f;
        }

        if (creditsTextRect != null) originalCreditsPos = creditsTextRect.anchoredPosition;

        if (loreCanvasGroup != null)
        {
            loreCanvasGroup.alpha = 0f;
            loreCanvasGroup.blocksRaycasts = false;
            loreCanvasGroup.gameObject.SetActive(false);
        }

        float masterVol = PlayerPrefs.GetFloat("MasterVol", 1f);
        float musicVol = PlayerPrefs.GetFloat("BGMVol", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVol", 1f);

        if (masterSlider != null) masterSlider.value = masterVol;
        if (musicSlider != null) musicSlider.value = musicVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateVolume(masterVol, musicVol, sfxVol);
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
            if (loreText != null) loreText.maxVisibleCharacters = 0;

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
                int totalChars = loreText.textInfo.characterCount;
                for (int i = 0; i <= totalChars; i++)
                {
                    loreText.maxVisibleCharacters = i;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            yield return new WaitForSeconds(timeToReadLore);
        }

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.SwitchScene(firstSceneName, () => Time.timeScale = 1f);
        else
            SceneManager.LoadScene(firstSceneName);
    }

    public void ContinueGame()
    {
        if (isStartingGame) return;
        if (SaveManager.Instance != null && SaveManager.Instance.IsSaveValid())
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

    public void QuitGame() => Application.Quit();

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        StartCoroutine(FadePanel(0f, 1f, 0.9f, 1f));
    }

    public void CloseSettings() => StartCoroutine(FadePanel(1f, 0f, 1f, 0.9f, true));

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
        creditsTextRect.anchoredPosition = new Vector2(originalCreditsPos.x, creditsStartY);
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
        if (disableAfter) settingsPanel.SetActive(false);
    }

    public void SetMasterVolume(float vol)
    {
        PlayerPrefs.SetFloat("MasterVol", vol);
        PlayerPrefs.Save();
        SyncAllAudio();
    }

    public void SetMusicVolume(float vol)
    {
        PlayerPrefs.SetFloat("BGMVol", vol);
        PlayerPrefs.Save();
        SyncAllAudio();
    }

    public void SetSFXVolume(float vol)
    {
        PlayerPrefs.SetFloat("SFXVol", vol);
        PlayerPrefs.Save();
        SyncAllAudio();
    }

    private void SyncAllAudio()
    {
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float music = PlayerPrefs.GetFloat("BGMVol", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateVolume(master, music, sfx);

        foreach (var ctrl in FindObjectsByType<AudioVolumeController>(FindObjectsSortMode.None))
        {
            ctrl.ApplyVolume();
        }
    }
}       