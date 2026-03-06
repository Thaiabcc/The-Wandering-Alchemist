using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        var systems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();

        if (systems.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}