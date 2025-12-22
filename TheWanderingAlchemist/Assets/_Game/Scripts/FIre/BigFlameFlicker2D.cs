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
    public float jitterAmount = 0.15f; // Khoảng cách tim lửa nhảy nhót
    public float speed = 8f;

    private Vector3 originalPosition;
    private float seed;

    void Start()
    {
        if (fireLight == null) fireLight = GetComponent<Light2D>();
        originalPosition = transform.localPosition;

        // Tạo một số ngẫu nhiên để các ngọn lửa không nhảy giống hệt nhau
        seed = Random.value * 100f;
    }

    void Update()
    {
        if (fireLight == null) return;

        float time = Time.time * speed + seed;

        // 1. Cường độ ánh sáng mượt mà hơn với PerlinNoise
        float noiseIntensity = Mathf.PerlinNoise(time, 0);
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noiseIntensity);

        // 2. Thay đổi bán kính tỏa sáng (Giúp lửa trông bập bùng lan tỏa)
        float noiseRadius = Mathf.PerlinNoise(0, time);
        fireLight.pointLightOuterRadius = Mathf.Lerp(minRadius, maxRadius, noiseRadius);

        // 3. Rung động vị trí (Tim lửa nhảy nhót)
        // Dùng PerlinNoise để vị trí dịch chuyển không quá gắt
        float offsetX = (Mathf.PerlinNoise(time * 1.5f, seed) - 0.5f) * jitterAmount;
        float offsetY = (Mathf.PerlinNoise(seed, time * 1.5f) - 0.5f) * jitterAmount;

        transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
    }
}