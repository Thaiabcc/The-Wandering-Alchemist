using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject canvasObject; 

    [Header("Settings")]
    [SerializeField] private bool hideOnDeath = true;
    [SerializeField] private bool hideWhenFull = false;

    private void Start()
    {
        if (hideWhenFull)
        {
            SetVisible(false);
        }
        else
        {
            SetVisible(true);
        }
    }
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0) return;
        float fillRatio = Mathf.Clamp01(currentHealth / maxHealth);
        if (fillImage != null)
        {
            fillImage.fillAmount = fillRatio;
        }

        HandleVisibility(currentHealth, maxHealth);
    }

    private void HandleVisibility(float current, float max)
    {
        if (current <= 0 && hideOnDeath)
        {
            SetVisible(false);
            return;
        }

        if (current >= max && hideWhenFull)
        {
            SetVisible(false);
            return;
        }
        SetVisible(true);
    }

    private void SetVisible(bool isActive)
    {
        if (canvasObject != null && canvasObject.activeSelf != isActive)
        {
            canvasObject.SetActive(isActive);
        }
    }
}