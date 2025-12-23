using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton để Boss gọi dễ dàng

    private Vector3 originalPos;
    private float shakeTimer;
    private float shakeAmount;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            // Rung lắc vị trí ngẫu nhiên trong vòng tròn nhỏ
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0f;
            transform.localPosition = originalPos; // Trả về vị trí cũ
        }
    }

    // Hàm để Boss gọi: Shake(Thời gian rung, Độ mạnh)
    public void Shake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeAmount = magnitude;
    }
}