using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Cài đặt Cổng")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Vector3 targetPosition; // Vị trí muốn đến ở map kia

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Gửi vị trí mong muốn cho GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.nextSpawnPosition = targetPosition;
            }

            // 2. Chuyển cảnh
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}