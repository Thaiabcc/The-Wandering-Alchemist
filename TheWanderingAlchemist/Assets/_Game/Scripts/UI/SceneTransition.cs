using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.9f;

    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;

    [SerializeField] private List<string> gameTips = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (loadingSlider != null) loadingSlider.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);
    }

    public void SwitchScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    public void SwitchSceneFromDeath(string sceneName)
    {
        StartCoroutine(TransitionFromDeathRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        ChangeRandomTip();
        yield return Fade(1);

        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0;
        }
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "Loading... 0%";
        }
        if (tipText != null) tipText.gameObject.SetActive(true);

        System.GC.Collect();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float targetProgress = 0f;
        while (operation.progress < 0.9f)
        {
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingSlider != null)
            {
                loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, targetProgress, Time.deltaTime * 3f);
                UpdateProgressText(loadingSlider.value);
            }
            yield return null;
        }

        while (loadingSlider.value < 1f)
        {
            loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, 1f, Time.deltaTime * 3f);
            UpdateProgressText(loadingSlider.value);
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        operation.allowSceneActivation = true;

        while (!operation.isDone) yield return null;

        yield return new WaitForSeconds(0.2f);

        if (loadingSlider != null) loadingSlider.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);

        yield return Fade(0);
    }

    private IEnumerator TransitionFromDeathRoutine(string sceneName)
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;

        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0;
        }
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "Loading... 0%";
        }
        if (tipText != null)
        {
            tipText.gameObject.SetActive(true);
            ChangeRandomTip();
        }

        System.GC.Collect();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float targetProgress = 0f;
        while (operation.progress < 0.9f)
        {
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingSlider != null)
            {
                loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, targetProgress, Time.deltaTime * 3f);
                UpdateProgressText(loadingSlider.value);
            }
            yield return null;
        }

        while (loadingSlider.value < 1f)
        {
            loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, 1f, Time.deltaTime * 3f);
            UpdateProgressText(loadingSlider.value);
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        operation.allowSceneActivation = true;

        while (!operation.isDone) yield return null;

        yield return new WaitForSeconds(0.2f);

        if (loadingSlider != null) loadingSlider.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);

        yield return Fade(0);
    }

    private void UpdateProgressText(float value)
    {
        if (progressText != null)
        {
            int percentage = Mathf.RoundToInt(value * 100f);
            progressText.text = $"Loading... {percentage}%";
        }
    }

    private void ChangeRandomTip()
    {
        if (tipText != null && gameTips.Count > 0)
        {
            int randomIndex = Random.Range(0, gameTips.Count);
            tipText.text = gameTips[randomIndex];
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0)
            fadeCanvasGroup.blocksRaycasts = false;
    }
}