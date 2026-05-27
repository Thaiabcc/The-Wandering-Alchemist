using UnityEngine;

public class Scroll : MonoBehaviour
{
    public float speed = 3f;
    private float currentSpeed;
    
    [Header("Move to Left")]
    public bool moveLeft = false;

    [Header("Position")]
    public float startX; 
    public float endX; 

    private void Start()
    {
        currentSpeed = speed;
    }

    private void Update()
    {
        if (currentSpeed == 0f) return;

        Vector3 direction = moveLeft ? Vector3.left : Vector3.right;
        transform.Translate(direction * currentSpeed * Time.deltaTime);

        if (moveLeft)
        {
            if (transform.position.x <= endX) 
            {
                ResetPosition();
            }
        }
        else
        {
            if (transform.position.x >= endX) 
            {
                ResetPosition();
            }
        }
    }

    private void ResetPosition()
    {
        transform.position = new Vector3(startX, transform.position.y, transform.position.z);
    }

    public void SetSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
}