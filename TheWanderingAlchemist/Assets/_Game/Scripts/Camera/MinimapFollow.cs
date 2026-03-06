using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Kéo nhân vật vào đây

    private void LateUpdate() // Dùng LateUpdate để camera đi mượt hơn
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;

            // Giữ nguyên độ cao Z của Camera (thường là -10) 
            // Nếu không chỉnh cái này camera sẽ chui vào trong đất
            newPosition.z = transform.position.z;

            transform.position = newPosition;

            // Nếu bro muốn map KHÔNG xoay theo nhân vật (Map cố định hướng Bắc)
            transform.rotation = Quaternion.Euler(0, 0, 0);

            // Nếu muốn map XOAY theo nhân vật thì xóa dòng trên đi và dùng:
            // transform.rotation = Quaternion.Euler(0, 0, player.eulerAngles.z);
        }
    }
}