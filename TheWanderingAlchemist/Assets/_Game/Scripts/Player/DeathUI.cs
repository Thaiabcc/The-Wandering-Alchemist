using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 

public class DeathUI : MonoBehaviour
{
    public static DeathUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject deathPanel;   
    public CanvasGroup canvasGroup;  
    public TextMeshProUGUI deadText; 

    private void Awake()
    {
        Instance = this;
        if (deathPanel) deathPanel.SetActive(false);
    }
    public IEnumerator FadeInBlack(float duration)
    {
        if (deathPanel) deathPanel.SetActive(true);
        if (deadText) deadText.gameObject.SetActive(true);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            if (canvasGroup) canvasGroup.alpha = t;
            yield return null;
        }
    }

    public IEnumerator FadeOutBlack(float duration)
    {
        if (deadText) deadText.gameObject.SetActive(false);

        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / duration;
            if (canvasGroup) canvasGroup.alpha = t;
            yield return null;
        }

        if (deathPanel) deathPanel.SetActive(false);
    }
}