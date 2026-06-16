using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public enum WeatherState { Sunny, Rainy, Stormy }

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    private WeatherAudio weatherAudio;

    [Header("Weather Particle Systems")]
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private ParticleSystem stormParticles;

    [Header("Light Modification")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color rainLightColor = new Color(0.5f, 0.6f, 0.7f, 1f);
    [SerializeField] private AnimationCurve intensityCurve;

    public bool isLightningFlashing = false;
    public WeatherState CurrentWeather { get; private set; } = WeatherState.Sunny;

    private Color originalLightColor = Color.white;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        weatherAudio = GetComponent<WeatherAudio>();

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        GameObject lightObj = GameObject.FindGameObjectWithTag("Light");
        if (lightObj != null)
        {
            globalLight = lightObj.GetComponent<Light2D>();
            if (globalLight != null)
                originalLightColor = globalLight.color;
        }

        GameObject rainObj = GameObject.FindGameObjectWithTag("Rain");
        if (rainObj != null) rainParticles = rainObj.GetComponent<ParticleSystem>();

        GameObject stormObj = GameObject.FindGameObjectWithTag("Stormy");
        if (stormObj != null) stormParticles = stormObj.GetComponent<ParticleSystem>();

        StartCoroutine(ApplyWeatherDelayed());
    }

    private IEnumerator ApplyWeatherDelayed()
    {
        yield return null;
        ChangeWeather(CurrentWeather);
    }

    private void Start()
    {
        if (globalLight != null)
            originalLightColor = globalLight.color;

        ChangeWeather(WeatherState.Sunny);
    }

    private Coroutine lightningCoroutine; // thêm field này

    public void ChangeWeather(WeatherState newState)
    {
        CurrentWeather = newState;
        StopAllParticles();

        // Chỉ stop lightning riêng, không StopAllCoroutines()
        if (lightningCoroutine != null)
        {
            StopCoroutine(lightningCoroutine);
            lightningCoroutine = null;
        }

        isLightningFlashing = false;

        switch (CurrentWeather)
        {
            case WeatherState.Sunny:
                ResetLightColor();
                weatherAudio?.StopLoop();
                break;

            case WeatherState.Rainy:
                ApplyRainyLight();
                weatherAudio?.PlayRain();
                if (rainParticles != null) rainParticles.Play();
                break;

            case WeatherState.Stormy:
                ApplyRainyLight();
                weatherAudio?.PlayStorm();
                if (stormParticles != null) stormParticles.Play();
                lightningCoroutine = StartCoroutine(LightningRoutine()); // lưu reference
                break;
        }
    }

    private void StopAllParticles()
    {
        if (rainParticles != null) rainParticles.Stop();
        if (stormParticles != null) stormParticles.Stop();
    }

    private void ApplyRainyLight()
    {
        if (globalLight != null)
            globalLight.color = originalLightColor * rainLightColor;
    }

    private void ResetLightColor()
    {
        if (globalLight != null)
            globalLight.color = originalLightColor;
    }

    private IEnumerator LightningRoutine()
    {
        while (CurrentWeather == WeatherState.Stormy)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            weatherAudio?.PlayThunder();

            if (globalLight != null)
            {
                float normalIntensity = globalLight.intensity;
                isLightningFlashing = true;

                globalLight.intensity = 4f;
                yield return new WaitForSeconds(0.06f);
                globalLight.intensity = normalIntensity;
                yield return new WaitForSeconds(0.04f);
                globalLight.intensity = 2.5f;
                yield return new WaitForSeconds(0.06f);
                globalLight.intensity = normalIntensity;

                isLightningFlashing = false;
            }
        }

        lightningCoroutine = null; // tự cleanup
    }

    public void LoadWeatherData(string savedWeatherState)
    {
        if (System.Enum.TryParse(savedWeatherState, out WeatherState loadedState))
        {
            ChangeWeather(loadedState);
        }
    }
}