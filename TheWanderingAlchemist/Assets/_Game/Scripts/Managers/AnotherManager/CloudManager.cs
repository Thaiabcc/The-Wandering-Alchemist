using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public static CloudManager Instance { get; private set; }

    [Header("Particle System")]
    [SerializeField] private ParticleSystem groundShadowClouds; 

    [Header("Cloud Settings")]
    [SerializeField] private float sunnySpeed = 1f;
    [SerializeField] private float stormSpeed = 4f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        HandleCloudVisuals();
    }

    private void HandleCloudVisuals()
    {
        if (TimeManager.Instance == null || WeatherManager.Instance == null || groundShadowClouds == null) return;

        float currentHour = TimeManager.Instance.CurrentHour;
        bool isMainDaytime = currentHour >= 7f && currentHour <= 17f;

        var shadowMain = groundShadowClouds.main;
        var shadowEmission = groundShadowClouds.emission;

        if (WeatherManager.Instance.CurrentWeather == WeatherState.Stormy)
        {
            shadowMain.simulationSpeed = stormSpeed;
            shadowEmission.enabled = false; 
        }
        else if (WeatherManager.Instance.CurrentWeather == WeatherState.Rainy)
        {
            shadowMain.simulationSpeed = sunnySpeed * 1.5f;
            shadowEmission.enabled = isMainDaytime;
        }
        else
        {
            shadowMain.simulationSpeed = sunnySpeed;
            shadowEmission.enabled = isMainDaytime;
        }
    }
}