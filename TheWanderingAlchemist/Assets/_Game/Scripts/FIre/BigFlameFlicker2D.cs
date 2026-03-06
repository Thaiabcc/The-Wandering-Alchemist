using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BigFlameFlicker2D : MonoBehaviour
{
    public Light2D fireLight;

    [Header("Cường độ (Intensity)")]
    public float minIntensity = 1.2f;
    public float maxIntensity = 2.5f;

    [Header("Phạm vi tỏa sáng (Radius)")]
    public float minRadius = 4f;
    public float maxRadius = 5.5f;

    [Header("Độ rung động (Jitter)")]
    public float jitterAmount = 0.15f;
    public float speed = 8f;

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

        float time = Time.time * speed + seed;
        float noiseIntensity = Mathf.PerlinNoise(time, 0);
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noiseIntensity);

        float noiseRadius = Mathf.PerlinNoise(0, time);
        fireLight.pointLightOuterRadius = Mathf.Lerp(minRadius, maxRadius, noiseRadius);

        float offsetX = (Mathf.PerlinNoise(time * 1.5f, seed) - 0.5f) * jitterAmount;
        float offsetY = (Mathf.PerlinNoise(seed, time * 1.5f) - 0.5f) * jitterAmount;

        transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
    }
}