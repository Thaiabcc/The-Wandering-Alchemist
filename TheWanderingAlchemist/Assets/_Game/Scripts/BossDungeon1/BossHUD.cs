using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    [Header("Thanh Máu")]
    public Slider hpSlider;

    [Header("Thanh Poise")]
    public Slider poiseSlider;
    public void SetMaxStats(float maxHP, float maxPoise)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP;
        }

        if (poiseSlider != null)
        {
            poiseSlider.maxValue = maxPoise;
            poiseSlider.value = maxPoise;
        }

        gameObject.SetActive(true);
    }

    public void UpdateHP(float currentHP)
    {
        if (hpSlider != null) hpSlider.value = currentHP;
    }

    public void UpdatePoise(float currentPoise)
    {
        if (poiseSlider != null) poiseSlider.value = currentPoise;
    }
}