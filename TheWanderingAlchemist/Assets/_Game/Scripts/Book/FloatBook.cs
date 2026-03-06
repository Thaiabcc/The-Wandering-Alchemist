using UnityEngine;

public class FloatBook : MonoBehaviour
{
    public Transform bookVisual;
    public SpriteRenderer shadowRenderer;

    [Header("Float Motion")]
    public float floatHeight = 0.12f;
    public float floatSpeed = 1.2f;

    [Header("Shadow")]
    public Transform shadow;
    public Vector3 shadowOffset = new Vector3(0, -0.15f, 0);
    public float minShadowScale = 0.75f; 
    public float maxShadowScale = 1.05f; 
    public float minShadowAlpha = 0.18f;
    public float maxShadowAlpha = 0.28f;

    float time;
    Vector3 startPos;

    void Start()
    {
        startPos = bookVisual.localPosition;
        shadow.localPosition = shadowOffset;
    }

    void Update()
    {
        time += Time.deltaTime;

        float sin = Mathf.Sin(time * floatSpeed);
        float yOffset = sin * floatHeight;
        bookVisual.localPosition = startPos + new Vector3(0, yOffset, 0);

        float height01 = (sin + 1f) * 0.5f;

        float shadowScale = Mathf.Lerp(maxShadowScale, minShadowScale, height01);
        shadow.localScale = new Vector3(shadowScale, shadowScale, 1f);

        float alpha = Mathf.Lerp(maxShadowAlpha, minShadowAlpha, height01);
        shadowRenderer.color = new Color(0f, 0f, 0f, alpha);
    }
}
