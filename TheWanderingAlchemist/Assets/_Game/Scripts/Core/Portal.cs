using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Dungeon Gate")]
    [SerializeField] private string sceneToLoad;      
    [SerializeField] private Vector3 targetPosition; 

    [Header("Return Set")]
    [SerializeField] private Transform returnPoint;
    public void Interact()
    {
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