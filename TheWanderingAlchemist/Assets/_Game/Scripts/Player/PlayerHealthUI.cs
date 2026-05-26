using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private RectTransform healthBarRect;
    [SerializeField] private float basePixelsPerHealth = 2.5f;  

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;

    [Header("Stamina Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    private float originalBarWidth;   

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (healthBarRect != null)
            originalBarWidth = healthBarRect.sizeDelta.x;
    }

    public void UpdateHealth(int current, int max)
    {
        if (!healthFill || !healthBarRect) return;

        float newWidth = originalBarWidth * (max / 100f); 

        healthBarRect.sizeDelta = new Vector2(newWidth, healthBarRect.sizeDelta.y);
        healthFill.fillAmount = (float)current / max;
    }

    public void UpdateStamina(float current, float max)
    {
        if (!staminaFill) return;

        float ratio = current / max;
        staminaFill.fillAmount = ratio;

        if (ratio < 0.15f)
            staminaFill.color = criticalColor;
        else if (ratio <= 0.5f)
            staminaFill.color = warningColor;
        else
            staminaFill.color = normalColor;
    }

    public void ResetHealthBar()
    {
        if (healthBarRect != null)
            healthBarRect.sizeDelta = new Vector2(originalBarWidth, healthBarRect.sizeDelta.y);
    }
}