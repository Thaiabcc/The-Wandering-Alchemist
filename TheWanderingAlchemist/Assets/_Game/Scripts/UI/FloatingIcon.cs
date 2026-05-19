using UnityEngine;

public class FloatingIcon : MonoBehaviour
{
    [Header("--- FLOATING EFFECT ---")]
    public float moveSpeed = 3f;      
    public float moveDistance = 0.15f; 

    [Header("--- BREATHING EFFECT (OPTIONAL) ---")]
    public bool enableScale = true;    
    public float scaleSpeed = 2f;
    public float scaleAmount = 0.05f;  

    private Vector3 startPos;
    private Vector3 startScale;

    private void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

        if (enableScale)
        {
            float scaleMultiplier = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
            transform.localScale = startScale * scaleMultiplier;
        }
    }
}