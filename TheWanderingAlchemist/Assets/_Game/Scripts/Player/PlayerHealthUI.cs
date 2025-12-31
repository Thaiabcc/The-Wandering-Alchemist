using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private Image healthFill;

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;

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

        staminaFill.fillAmount = current / max;
    }
}
