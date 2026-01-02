using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    // Biến lưu trữ tạm camera hiện tại
    private CinemachineVirtualCamera currentVcam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Bất tử
    }

    public void Shake(float duration, float intensity)
    {
        // 1. Tự động đi tìm Virtual Camera đang hoạt động trong Scene hiện tại
        // (Cách này giúp bro không cần quan tâm camera cũ hay mới)
        if (currentVcam == null || !currentVcam.gameObject.activeInHierarchy)
        {
            currentVcam = FindObjectOfType<CinemachineVirtualCamera>();
        }

        // Nếu tìm mãi không thấy (ví dụ scene Menu không có cam) thì bỏ qua
        if (currentVcam == null) return;

        // 2. Lấy Noise
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

            // Logic giảm dần độ rung
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