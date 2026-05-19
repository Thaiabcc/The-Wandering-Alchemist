using UnityEngine;
using UnityEngine.Rendering.Universal; 
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float timeScale = 60f; 
    [SerializeField] private int startHour = 6;    

    [Header("Light References")]
    [SerializeField] private Light2D globalLight;   
    [SerializeField] private Gradient dayNightGradient; 

    // Biến lưu trữ thời gian thực tế trong game
    public int CurrentDay { get; private set; } = 1;
    public float CurrentHour { get; private set; }
    public float CurrentMinute { get; private set; }

    private float totalSecondsInDay;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CurrentHour = startHour;
        totalSecondsInDay = (CurrentHour * 3600f) + (CurrentMinute * 60f);
    }

    private void Update()
    {
        totalSecondsInDay += Time.deltaTime * timeScale;
        CurrentHour = Mathf.FloorToInt(totalSecondsInDay / 3600f);
        CurrentMinute = Mathf.FloorToInt((totalSecondsInDay % 3600f) / 60f);
        UpdateDayNightLight();
        if (totalSecondsInDay >= 86400f)
        {
            StartNewDay();
        }
    }

    private void UpdateDayNightLight()
    {
        if (globalLight == null || dayNightGradient == null) return;
        float percentageOfDay = totalSecondsInDay / 86400f;
        globalLight.color = dayNightGradient.Evaluate(percentageOfDay);
    }

    private void StartNewDay()
    {
        totalSecondsInDay = 0f;
        CurrentHour = 0;
        CurrentMinute = 0;
        CurrentDay++;

        Debug.Log("Đã bước sang ngày thứ: " + CurrentDay);
    }

    public string GetTimeString()
    {
        return string.Format("{0:00}:{1:00}", CurrentHour, CurrentMinute);
    }
}