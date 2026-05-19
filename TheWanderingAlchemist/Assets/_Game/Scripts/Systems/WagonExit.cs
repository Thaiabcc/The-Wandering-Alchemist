using UnityEngine;
using UnityEngine.SceneManagement;

public class WagonExit : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        string sceneToLoad = GameManager.Instance.lastWorldScene;
        Vector3 posToSpawn = GameManager.Instance.lastWorldPosition;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneToLoad = "Town_01";
            posToSpawn = new Vector3(5, 5, 0);
        }

        GameManager.Instance.nextSpawnPosition = posToSpawn;

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