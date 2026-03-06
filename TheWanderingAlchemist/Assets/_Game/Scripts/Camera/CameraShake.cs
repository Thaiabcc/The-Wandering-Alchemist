using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    private CinemachineVirtualCamera currentVcam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Shake(float duration, float intensity)
    {
        if (currentVcam == null || !currentVcam.gameObject.activeInHierarchy)
        {
            currentVcam = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (currentVcam == null) return;

        var noise = currentVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise != null)
        {
            noise.m_AmplitudeGain = intensity;
            startingIntensity = intensity;
            shakeTimerTotal = duration;
            shakeTimer = duration;
        }
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (currentVcam != null)
            {
                var noise = currentVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                if (noise != null)
                {
                    noise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
                }
            }
        }
    }
}