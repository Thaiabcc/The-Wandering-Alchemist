using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timeText; 
    [SerializeField] private TextMeshProUGUI dayText;  
    [SerializeField] private RectTransform clockDial;  

    private void Update()
    {
        if (TimeManager.Instance == null) return;

        if (timeText != null)
        {
            timeText.text = TimeManager.Instance.GetTimeString();
        }

        if (dayText != null)
        {
            dayText.text = "Day " + TimeManager.Instance.CurrentDay;
        }
        
        if (clockDial != null)
        {
            float totalMinutesInDay = (TimeManager.Instance.CurrentHour * 60f) + TimeManager.Instance.CurrentMinute;
            float dayPercentage = totalMinutesInDay / 1440f;
            float rotationAngle = dayPercentage * -360f;
            float finalZRotation = rotationAngle + 180f; 
            
            clockDial.localRotation = Quaternion.Euler(0, 0, finalZRotation);
        }
    }
}