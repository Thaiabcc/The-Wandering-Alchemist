using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class StoryTransitionUI : MonoBehaviour
{
    public static StoryTransitionUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject storyPanel;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI storyText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (storyPanel) storyPanel.SetActive(false);
    }
    public void PlayStory(string text, float duration = 3f, Action onComplete = null)
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            storyText.text = text;
            StartCoroutine(StorySequence(duration, onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator StorySequence(float waitTime, Action onComplete)
    {
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            if (canvasGroup) canvasGroup.alpha = t / 0.5f;
            yield return null;
        }
        if (canvasGroup) canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(waitTime);
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            if (canvasGroup) canvasGroup.alpha = 1f - (t / 0.5f);
            yield return null;
        }
        if (canvasGroup) canvasGroup.alpha = 0f;

        storyPanel.SetActive(false);
        onComplete?.Invoke();
    }
}