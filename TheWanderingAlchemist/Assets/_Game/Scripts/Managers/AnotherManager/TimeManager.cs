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
    [SerializeField] private AnimationCurve intensityCurve;

    public int CurrentDay { get; private set; } = 1;
    public float CurrentHour { get; private set; }
    public float CurrentMinute { get; private set; }

    private double accumSeconds;
    private int totalSecondsInDay;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CurrentHour = startHour;
        totalSecondsInDay = startHour * 3600;
        accumSeconds = totalSecondsInDay;
    }

    private void Update()
    {
        accumSeconds += Time.deltaTime * timeScale;
        totalSecondsInDay = (int)accumSeconds;

        CurrentHour = totalSecondsInDay / 3600;
        CurrentMinute = (totalSecondsInDay % 3600) / 60;

        UpdateDayNightLight();

        if (totalSecondsInDay >= 86400)
        {
            StartNewDay();
        }
    }

    private void UpdateDayNightLight()
    {
        if (globalLight == null || dayNightGradient == null) return;
        
        float percentageOfDay = (float)totalSecondsInDay / 86400f;
        globalLight.color = dayNightGradient.Evaluate(percentageOfDay);

        if (intensityCurve != null)
        {
            globalLight.intensity = intensityCurve.Evaluate(percentageOfDay);
        }
    }

    private void StartNewDay()
    {
        accumSeconds = 0;
        totalSecondsInDay = 0;
        CurrentHour = 0;
        CurrentMinute = 0;
        CurrentDay++;

        Debug.Log("Đã bước sang ngày thứ: " + CurrentDay);

        if (WeatherManager.Instance != null)
        {
            float chance = Random.value;
            if (chance < 0.6f)
                WeatherManager.Instance.ChangeWeather(WeatherState.Sunny);
            else if (chance < 0.85f)
                WeatherManager.Instance.ChangeWeather(WeatherState.Rainy);
            else
                WeatherManager.Instance.ChangeWeather(WeatherState.Stormy);
        }
    }

    public string GetTimeString()
    {
        return string.Format("{0:00}:{1:00}", CurrentHour, CurrentMinute);
    }
}