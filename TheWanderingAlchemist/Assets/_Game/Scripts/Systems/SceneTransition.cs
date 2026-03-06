using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Kéo cái Panel đen")]
    public CanvasGroup fadeCanvasGroup;

    [Header("Tốc độ chuyển cảnh")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void SwitchScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        yield return Fade(1);
        System.GC.Collect();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.8f);
        yield return Fade(0);
    }

    IEnumerator Fade(float targetAlpha)
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
        yield return new WaitForEndOfFrame();
        yield return null;
        if (targetAlpha == 0) fadeCanvasGroup.blocksRaycasts = false;
    }
}