using UnityEngine;

public class DynamicShadow : MonoBehaviour
{
    private SpriteRenderer shadowRenderer;

    [Header("Shadow Opacity Settings")]
    [SerializeField] private float maxAlpha = 0.6f;
    [SerializeField] private float minAlpha = 0.1f;
    [SerializeField] private Color shadowColor = Color.black;

    [Header("Wobble Effect Settings")]
    [SerializeField] private float wobbleSpeed = 5f;
    [SerializeField] private float wobbleAmount = 0.05f;

    private Vector3 originalScale;

    private void Awake()
    {
        shadowRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (shadowRenderer == null || TimeManager.Instance == null) return;

        float hour = TimeManager.Instance.CurrentHour;
        float intensityFactor = 1f - (Mathf.Abs(hour - 12f) / 12f);
        float targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, intensityFactor);

        Color finalColor = shadowColor;
        finalColor.a = targetAlpha;
        shadowRenderer.color = finalColor;

        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;

        transform.localScale = new Vector3(
            originalScale.x + wobble,
            originalScale.y - wobble,
            originalScale.z
        );
    }
}