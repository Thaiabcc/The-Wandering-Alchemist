using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Cài đặt Cổng Dungeon")]
    [SerializeField] private string sceneToLoad;      
    [SerializeField] private Vector3 targetPosition; 

    [Header("Cài đặt Hồi Hương")]
    [Tooltip("Empty Object (ReturnPoint)")]
    [SerializeField] private Transform returnPoint;
    public void Interact()
    {
        Debug.Log("Đang mở cổng...");

        GameManager.Instance.lastWorldScene = SceneManager.GetActiveScene().name;
        Vector3 myReturnPos = transform.position;
        if (returnPoint != null)
        {
            myReturnPos = returnPoint.position;
        }
        myReturnPos.z = 0; 

        GameManager.Instance.lastWorldPosition = myReturnPos;
        GameManager.Instance.nextSpawnPosition = targetPosition;
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}