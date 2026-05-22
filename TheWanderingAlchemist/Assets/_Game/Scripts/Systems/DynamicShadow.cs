using UnityEngine;

public class DynamicShadow : MonoBehaviour
{
    private SpriteRenderer shadowRenderer;

    [Header("Cấu Hình Độ Đậm Nhạt")]
    [SerializeField] private float maxAlpha = 0.6f; 
    [SerializeField] private float minAlpha = 0.1f; 
    [SerializeField] private Color shadowColor = Color.black; 

    [Header("Cấu Hướng Hiệu Ứng Rung (MỚI)")]
    [SerializeField] private float wobbleSpeed = 5f;    // Tốc độ rung (càng cao rung càng nhanh)
    [SerializeField] private float wobbleAmount = 0.05f; // Độ mạnh của độ rung (biên độ co giãn)

    private Vector3 originalScale;

    private void Awake()
    {
        shadowRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (shadowRenderer == null || TimeManager.Instance == null) return;

        // 1. Đồng bộ độ đậm nhạt theo TimeManager
        float hour = TimeManager.Instance.CurrentHour;
        float intensityFactor = 1f - (Mathf.Abs(hour - 12f) / 12f); 
        float targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, intensityFactor);

        // 2. Ép màu sắc theo cấu hình ngoài Inspector
        Color finalColor = shadowColor;
        finalColor.a = targetAlpha;
        shadowRenderer.color = finalColor;

        // 3. Tạo hiệu ứng rung rinh co giãn nhẹ (Fake gió thổi lá cây)
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        transform.localScale = new Vector3(originalScale.x + wobble, originalScale.y - wobble, originalScale.z);
    }
}