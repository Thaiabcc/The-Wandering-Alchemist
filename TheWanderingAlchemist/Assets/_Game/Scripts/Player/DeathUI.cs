using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 

public class DeathUI : MonoBehaviour
{
    public static DeathUI Instance { get; private set; }

    public GameObject deathPanel;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI deadText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetUI();
    }

    public void ResetUI()
    {
        StopAllCoroutines();
        if (canvasGroup) canvasGroup.alpha = 0f;
        if (deathPanel) deathPanel.SetActive(false);
        if (deadText) deadText.gameObject.SetActive(false);
    }

    public IEnumerator FadeInBlack(float duration)
    {
        ResetUI(); // đảm bảo sạch

        if (deathPanel) deathPanel.SetActive(true);
        if (deadText) deadText.gameObject.SetActive(true);

        if (canvasGroup)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }

    public IEnumerator FadeOutBlack(float duration)
    {
        Debug.Log("FadeOutBlack started");
    
        if (deadText) deadText.gameObject.SetActive(false);

        if (canvasGroup)
        {
            float start = canvasGroup.alpha;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                canvasGroup.alpha = Mathf.Lerp(start, 0, t);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        if (deathPanel) deathPanel.SetActive(false);
    
        Debug.Log("FadeOutBlack finished");
    }
}