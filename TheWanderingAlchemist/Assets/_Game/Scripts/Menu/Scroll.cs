using UnityEngine;

public class Scroll : MonoBehaviour
{
    public float speed = 3f;
    
    [Header("Move to Left")]
    public bool moveLeft = false;

    [Header("Position")]
    public float startX; 
    public float endX; 

    void Update()
    {
        Vector3 direction = moveLeft ? Vector3.left : Vector3.right;
        transform.Translate(direction * speed * Time.deltaTime);

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

    void ResetPosition()
    {
        transform.position = new Vector3(startX, transform.position.y, transform.position.z);
    }
}