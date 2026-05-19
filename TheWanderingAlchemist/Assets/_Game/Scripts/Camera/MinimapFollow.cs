using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; 

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}