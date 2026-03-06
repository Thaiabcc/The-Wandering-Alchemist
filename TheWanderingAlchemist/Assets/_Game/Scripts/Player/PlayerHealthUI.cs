using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private Image healthFill;

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;

    [Header("Stamina Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 1f, 0.2f); 
    [SerializeField] private Color warningColor = Color.yellow;             
    [SerializeField] private Color criticalColor = Color.red;               

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ------------------ HEALTH ------------------

    public void UpdateHealth(int current, int max)
    {
        if (!healthFill) return;

        healthFill.fillAmount = (float)current / max;
    }

    // ------------------ STAMINA ------------------

    public void UpdateStamina(float current, float max)
    {
        if (!staminaFill) return;

        // 1. Caculate %
        float ratio = current / max;
        staminaFill.fillAmount = ratio;

        // 2. Color Changing
        if (ratio < 0.15f) // < 15%
        {
            staminaFill.color = criticalColor;
        }
        else if (ratio <= 0.5f) // <15% < 50%
        {
            staminaFill.color = warningColor;
        }
        else // >50%
        {
            staminaFill.color = normalColor;
        }
    }
}