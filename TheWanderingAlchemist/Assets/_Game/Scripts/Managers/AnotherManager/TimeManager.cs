using UnityEngine;
using UnityEngine.Rendering.Universal; 
using UnityEngine.SceneManagement; 
using System;
using Random = UnityEngine.Random;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<bool> OnNightStateChanged; 

    [Header("Time Settings")]
    [SerializeField] private float timeScale = 60f; 
    [SerializeField] private int startHour = 6;    

    [Header("Light References")]
    [SerializeField] private Light2D globalLight;   
    [SerializeField] private Gradient dayNightGradient; 
    [SerializeField] private AnimationCurve intensityCurve;

    [Header("Light Automation Settings")]
    [SerializeField] private int turnOnLightsHour = 18;  
    [SerializeField] private int turnOffLightsHour = 6;  

    public int CurrentDay { get; private set; } = 1;
    public float CurrentHour { get; private set; }
    public float CurrentMinute { get; private set; }

    private bool isNight = false; 
    public bool IsNight => isNight; 

    private double accumSeconds;
    private int totalSecondsInDay;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSpecificGlobalLight();
    }

    private void Start()
    {
        FindSpecificGlobalLight();

        CurrentHour = startHour;
        totalSecondsInDay = startHour * 3600;
        accumSeconds = totalSecondsInDay;
        isNight = CheckIfNight(CurrentHour);
    }

    private void FindSpecificGlobalLight()
    {
        globalLight = null;

        GameObject lightObj = GameObject.Find("GlobalLight_Ambient");
        if (lightObj != null)
        {
            globalLight = lightObj.GetComponent<Light2D>();
        }
    }

    private void Update()
    {
        accumSeconds += Time.deltaTime * timeScale;
        totalSecondsInDay = (int)accumSeconds;

        CurrentHour = totalSecondsInDay / 3600;
        CurrentMinute = (totalSecondsInDay % 3600) / 60;

        UpdateDayNightLight();
        CheckLightAutomation(); 

        if (totalSecondsInDay >= 86400)
        {
            StartNewDay();
        }
    }

    private void CheckLightAutomation()
    {
        bool currentNightState = CheckIfNight(CurrentHour);

        if (currentNightState != isNight)
        {
            isNight = currentNightState;
            OnNightStateChanged?.Invoke(isNight);
        }
    }

    private bool CheckIfNight(float hour)
    {
        return (hour >= turnOnLightsHour || hour < turnOffLightsHour);
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
    
    public void LoadTimeData(int savedDay, double savedAccumSeconds)
    {
        CurrentDay = savedDay;
        accumSeconds = savedAccumSeconds;
        totalSecondsInDay = (int)accumSeconds;

        CurrentHour = totalSecondsInDay / 3600;
        CurrentMinute = (totalSecondsInDay % 3600) / 60;

        UpdateDayNightLight();
        isNight = CheckIfNight(CurrentHour);
    }
}