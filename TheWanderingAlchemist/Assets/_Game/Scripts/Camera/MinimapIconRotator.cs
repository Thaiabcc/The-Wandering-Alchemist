using UnityEngine;

public class MinimapIconRotator : MonoBehaviour
{
    public float rotationSpeed = 20f;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movementDirection = new Vector2(moveX, moveY);
        if (movementDirection.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(movementDirection.y, -movementDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}