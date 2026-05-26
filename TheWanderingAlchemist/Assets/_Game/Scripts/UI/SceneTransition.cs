using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.9f;

    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private List<string> gameTips = new List<string>();

    [Header("Loading Settings")]
    [SerializeField] private float loadingSpeed = 1.5f;       

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

    public void SwitchScene(string sceneName, Action onLoaded = null)
    {
        StartCoroutine(TransitionRoutine(sceneName, onLoaded));
    }

    public void SwitchSceneFromDeath(string sceneName)
    {
        StartCoroutine(TransitionFromDeathRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName, Action onLoaded = null)
    {
        ChangeRandomTip();
        yield return Fade(1);

        ActivateLoadingUI();
        
        System.GC.Collect();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        yield return RunLoadingBar(operation);

        yield return new WaitForSeconds(0.3f);
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;

        yield return new WaitForSeconds(0.2f);

        onLoaded?.Invoke();
        DeactivateLoadingUI();
        yield return Fade(0);
    }

    private IEnumerator TransitionFromDeathRoutine(string sceneName)
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;

        ActivateLoadingUI();
        ChangeRandomTip();
        
        System.GC.Collect();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        yield return RunLoadingBar(operation);

        yield return new WaitForSeconds(0.3f);
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;

        yield return new WaitForSeconds(0.2f);

        DeactivateLoadingUI();
        yield return Fade(0);
    }

    private void ActivateLoadingUI()
    {
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0f;
        }
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "Loading... 0%";
        }
        if (tipText != null) tipText.gameObject.SetActive(true);
    }

    private void DeactivateLoadingUI()
    {
        if (loadingSlider != null) loadingSlider.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);
    }

    private IEnumerator RunLoadingBar(AsyncOperation operation)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            float target = operation.progress < 0.9f ? operation.progress / 0.9f : 1f;
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime * loadingSpeed);

            if (loadingSlider != null)
                loadingSlider.value = progress;

            UpdateProgressText(progress);

            yield return null;
        }
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