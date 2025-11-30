using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    private const int TargetFPS = 120;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFPS;
        Debug.Log($"FPS Limiter: Set application target frame rate to {TargetFPS} FPS.");
    }
}