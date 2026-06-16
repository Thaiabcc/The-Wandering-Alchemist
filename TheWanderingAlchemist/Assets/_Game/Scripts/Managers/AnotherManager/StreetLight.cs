using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class StreetLight : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private SpriteRenderer lightOffSprite;
    [SerializeField] private SpriteRenderer lightOnSprite;
    [SerializeField] private Light2D light2D;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float maxIntensity = 1.5f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnNightStateChanged += HandleLightToggle;
            HandleLightToggle(TimeManager.Instance.IsNight);
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnNightStateChanged -= HandleLightToggle;
        }
    }

    private void HandleLightToggle(bool isNight)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(isNight));
    }

    private IEnumerator FadeRoutine(bool turnOn)
    {
        float startAlpha = lightOnSprite.color.a;
        float targetAlpha = turnOn ? 1f : 0f;

        float startIntensity = light2D.intensity;
        float targetIntensity = turnOn ? maxIntensity : 0f;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeDuration;

            Color onColor = lightOnSprite.color;
            onColor.a = Mathf.Lerp(startAlpha, targetAlpha, progress);
            lightOnSprite.color = onColor;

            Color offColor = lightOffSprite.color;
            offColor.a = 1f - onColor.a;
            lightOffSprite.color = offColor;

            light2D.intensity = Mathf.Lerp(
                startIntensity,
                targetIntensity,
                progress
            );

            yield return null;
        }

        Color finalOn = lightOnSprite.color;
        finalOn.a = targetAlpha;
        lightOnSprite.color = finalOn;

        Color finalOff = lightOffSprite.color;
        finalOff.a = 1f - targetAlpha;
        lightOffSprite.color = finalOff;

        light2D.intensity = targetIntensity;
    }
}