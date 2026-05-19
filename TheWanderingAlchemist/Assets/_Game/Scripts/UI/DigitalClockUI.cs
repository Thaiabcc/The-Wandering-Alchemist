using UnityEngine;
using TMPro;

public class DigitalClockUI : MonoBehaviour
{
    private TextMeshProUGUI clockText;

    private void Awake()
    {
        clockText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (TimeManager.Instance != null && clockText != null)
        {
            clockText.text = TimeManager.Instance.GetTimeString();
        }
    }
}