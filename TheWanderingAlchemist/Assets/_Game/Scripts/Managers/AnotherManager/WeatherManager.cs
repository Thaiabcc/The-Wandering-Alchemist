using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public enum WeatherState { Sunny, Rainy, Stormy }

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Weather Particle Systems")]
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private ParticleSystem stormParticles;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource weatherAudioSource;
    [SerializeField] private AudioClip rainSFX;
    [SerializeField] private AudioClip stormSFX;

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
    }

    private void Start()
    {
        if (globalLight != null)
            originalLightColor = globalLight.color;

        ChangeWeather(WeatherState.Rainy);
    }

    public void ChangeWeather(WeatherState newState)
    {
        CurrentWeather = newState;

        StopAllParticles();
        StopAllCoroutines();

        switch (CurrentWeather)
        {
            case WeatherState.Sunny:
                ResetLightColor();
                StopAudio();
                break;

            case WeatherState.Rainy:
                ApplyRainyLight();
                PlayAudio(rainSFX);

                if (rainParticles != null)
                    rainParticles.Play();

                break;

            case WeatherState.Stormy:
                ApplyRainyLight();
                PlayAudio(stormSFX);

                if (stormParticles != null)
                    stormParticles.Play();

                StartCoroutine(LightningRoutine());
                break;
        }
    }

    private void StopAllParticles()
    {
        if (rainParticles != null)
            rainParticles.Stop();

        if (stormParticles != null)
            stormParticles.Stop();
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

    private void PlayAudio(AudioClip clip)
    {
        if (weatherAudioSource == null || clip == null)
            return;

        weatherAudioSource.clip = clip;
        weatherAudioSource.loop = true;
        weatherAudioSource.Play();
    }

    private void StopAudio()
    {
        if (weatherAudioSource != null)
            weatherAudioSource.Stop();
    }

    private IEnumerator LightningRoutine()
    {
        while (CurrentWeather == WeatherState.Stormy)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));

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
    }
}