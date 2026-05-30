using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingManager : MonoBehaviour
{
    [Header("Cinematic Flow")]
    public CanvasGroup blackScreenCanvasGroup; 
    public string postGameScene = "Town1"; 
    public float fadeSpeed = 2f;
    public float timeBeforeLore = 4f;

    [Header("Lore Text Settings")]
    public CanvasGroup loreCanvasGroup;
    public TMP_Text loreText;
    public float typingSpeed = 0.05f;
    public float timeToRead = 4f;

    [Header("Credits Settings")]
    public CanvasGroup creditsCanvasGroup;
    public RectTransform creditsTextRect;
    public float creditsScrollSpeed = 100f; 
    
    [Tooltip("Chỉnh tọa độ Y bắt đầu của sớ. Càng âm thì sớ xuất phát càng sâu.")]
    public float startingYPos = -8000f; // <-- CÔNG TẮC CHO MÀY TỰ VẶN TỌA ĐỘ ĐÂY

    private Vector2 originalCreditsPos;

    void Start()
    {
        if (blackScreenCanvasGroup) blackScreenCanvasGroup.alpha = 1f;
        
        if (loreCanvasGroup) 
        {
            loreCanvasGroup.alpha = 0f;
            loreCanvasGroup.gameObject.SetActive(false);
        }
        
        if (creditsCanvasGroup) 
        {
            creditsCanvasGroup.alpha = 0f;
            creditsCanvasGroup.gameObject.SetActive(false);
        }
        
        if (creditsTextRect != null)
        {
            originalCreditsPos = creditsTextRect.anchoredPosition;
            // Áp dụng luôn cái số mày điền ngoài Unity vào đây
            creditsTextRect.anchoredPosition = new Vector2(originalCreditsPos.x, startingYPos);
        }

        StartCoroutine(PlayEndingSequence());
    }

    IEnumerator PlayEndingSequence()
    {
        yield return new WaitForSeconds(1f);
        yield return FadeCanvas(blackScreenCanvasGroup, 1f, 0f);
        yield return new WaitForSeconds(timeBeforeLore);

        if (loreCanvasGroup != null)
        {
            loreCanvasGroup.gameObject.SetActive(true);
            loreCanvasGroup.alpha = 1f;
            loreText.maxVisibleCharacters = 0;

            loreText.ForceMeshUpdate();
            int totalChars = loreText.textInfo.characterCount;
            int visibleCount = 0;

            while (visibleCount <= totalChars)
            {
                loreText.maxVisibleCharacters = visibleCount;
                visibleCount++;
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(timeToRead);

            yield return FadeCanvas(loreCanvasGroup, 1f, 0f);
            loreCanvasGroup.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f); 
        }

        if (creditsCanvasGroup != null)
        {
            creditsCanvasGroup.gameObject.SetActive(true); 
            creditsCanvasGroup.alpha = 1f;
            
            float targetY = creditsTextRect.rect.height + 1200f;
            
            while (creditsTextRect.anchoredPosition.y < targetY)
            {
                creditsTextRect.anchoredPosition += Vector2.up * creditsScrollSpeed * Time.deltaTime;
                yield return null;
            }
            
            yield return FadeCanvas(creditsCanvasGroup, 1f, 0f);
            creditsCanvasGroup.gameObject.SetActive(false);
        }

        yield return FadeCanvas(blackScreenCanvasGroup, 0f, 1f);
        yield return new WaitForSeconds(1f);

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(postGameScene, () => { Time.timeScale = 1f; });
        }
        else
        {
            SceneManager.LoadScene(postGameScene);
        }
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float start, float end)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            cg.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }
        cg.alpha = end;
    }
}