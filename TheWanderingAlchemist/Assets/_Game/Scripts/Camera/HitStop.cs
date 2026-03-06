using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private bool isStopping = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Stop(float duration)
    {
        if (isStopping) return; 
        StartCoroutine(StopRoutine(duration));
    }

    private IEnumerator StopRoutine(float duration)
    {
        isStopping = true;

        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        isStopping = false;
    }
}