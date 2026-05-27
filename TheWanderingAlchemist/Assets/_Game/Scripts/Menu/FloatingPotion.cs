using UnityEngine;

public class FloatingPotion : MonoBehaviour
{
    [Header("Floating Potion")]
    public float floatSpeed = 2f;    
    public float floatHeight = 15f; 

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}