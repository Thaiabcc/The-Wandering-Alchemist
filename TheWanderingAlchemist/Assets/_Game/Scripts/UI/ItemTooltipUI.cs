using UnityEngine;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    private static ItemTooltipUI _instance;
    public static ItemTooltipUI Instance 
    {
        get {
            if (_instance == null) _instance = FindObjectOfType<ItemTooltipUI>();
            return _instance;
        }
    }

    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start() 
    {
        Hide();
    }

    private void Update()
    {
        if (root.activeSelf)
        {
            transform.position = Input.mousePosition + new Vector3(20, -20, 0);
        }
    }

    public void Show(ItemData item)
    {
        if (item == null) return;
        
        itemNameText.text = item.itemName;
        itemTypeText.text = $"Type: {item.itemType}";
        descriptionText.text = item.description;
        
        root.SetActive(true);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(1f, 0.2f)); 
    }

    public void Hide()
    {
        if (root == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(0f, 0.1f, () => root.SetActive(false)));
    }

    private System.Collections.IEnumerator Fade(float targetAlpha, float duration, System.Action onComplete = null)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }
    
}