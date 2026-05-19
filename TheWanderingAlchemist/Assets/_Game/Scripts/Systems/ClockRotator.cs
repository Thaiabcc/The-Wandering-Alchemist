using UnityEngine;

public class ClockRotator : MonoBehaviour
{
    private RectTransform dialTransform;

    private void Awake()
    {
        dialTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (TimeManager.Instance == null || dialTransform == null) return;
        float currentHourDecimal = TimeManager.Instance.CurrentHour + (TimeManager.Instance.CurrentMinute / 60f);
        float rotationAngle = currentHourDecimal * -15f;
        dialTransform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
    }
}