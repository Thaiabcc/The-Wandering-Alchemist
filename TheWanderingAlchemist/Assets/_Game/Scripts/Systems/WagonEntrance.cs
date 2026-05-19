using UnityEngine;
using UnityEngine.SceneManagement;

public class WagonEntrance : MonoBehaviour, IInteractable
{
    [Header("Map Settings")]
    [SerializeField] private string interiorSceneName = "WagonInterior";

    [SerializeField] private Vector3 spawnPosInside = new Vector3(0, -2, 0);

    [Header("Return Position Settings")]
    [Tooltip("Return Point")]
    [SerializeField] private Transform returnPoint;

    public void Interact()
    {
        GameManager.Instance.lastWorldScene = SceneManager.GetActiveScene().name;

        Vector3 returnPos;

        if (returnPoint != null)
        {
            returnPos = returnPoint.position;
        }
        else
        {
            returnPos = transform.position + new Vector3(0, -2.0f, 0);
        }

        returnPos.z = 0;

        GameManager.Instance.lastWorldPosition = returnPos;
        GameManager.Instance.nextSpawnPosition = spawnPosInside;

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.SwitchScene(interiorSceneName);
        }
        else
        {
            SceneManager.LoadScene(interiorSceneName);
        }
    }
}