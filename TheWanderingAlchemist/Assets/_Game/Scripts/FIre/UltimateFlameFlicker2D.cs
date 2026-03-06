using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UltimateFlameFlicker2D : MonoBehaviour
{
    public Light2D fireLight;

    [Header("Màu sắc (Color)")]
    public Color hotColor = new Color(1f, 0.9f, 0.5f);
    public Color coldColor = new Color(1f, 0.3f, 0f);

    [Header("Cường độ (Intensity)")]
    public float minIntensity = 1.0f;
    public float maxIntensity = 2.2f;

    [Header("Độ đậm của bóng (Shadow)")] 
    [Range(0f, 1f)] public float minShadowAlpha = 0.5f; 
    [Range(0f, 1f)] public float maxShadowAlpha = 1.0f; 

    [Header("Phạm vi tỏa sáng (Radius)")]
    public float minRadius = 3.5f;
    public float maxRadius = 5.0f;

    [Header("Tốc độ & Độ rung")]
    public float speed = 8f;
    public float jitterAmount = 0.1f;

    private Vector3 originalPosition;
    private float seed;

    void Start()
    {
        if (fireLight == null) fireLight = GetComponent<Light2D>();
        originalPosition = transform.localPosition;
        seed = Random.value * 100f;
    }

    void Update()
    {
        if (fireLight == null) return;

        float noise = Mathf.PerlinNoise(Time.time * speed + seed, 0);

        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        fireLight.color = Color.Lerp(coldColor, hotColor, noise);

        fireLight.shadowIntensity = Mathf.Lerp(minShadowAlpha, maxShadowAlpha, noise);

        float radiusNoise = Mathf.PerlinNoise(0, Time.time * speed * 0.8f + seed);
        fireLight.pointLightOuterRadius = Mathf.Lerp(minRadius, maxRadius, radiusNoise);

        float offsetX = (Mathf.PerlinNoise(Time.time * speed * 1.5f, seed) - 0.5f) * jitterAmount;
        float offsetY = (Mathf.PerlinNoise(seed, Time.time * speed * 1.5f) - 0.5f) * jitterAmount;
        transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
    }
}