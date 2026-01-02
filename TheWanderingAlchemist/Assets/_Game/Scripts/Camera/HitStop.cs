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

    // Hàm gọi ngưng đọng: duration thường rất nhỏ (0.05f đến 0.1f)
    public void Stop(float duration)
    {
        if (isStopping) return; // Đang stop rồi thì thôi
        StartCoroutine(StopRoutine(duration));
    }

    private IEnumerator StopRoutine(float duration)
    {
        isStopping = true;

        // 1. Đóng băng thời gian
        Time.timeScale = 0.0f;

        // 2. Chờ (Phải dùng Realtime vì timeScale đang bằng 0)
        yield return new WaitForSecondsRealtime(duration);

        // 3. Trả lại thời gian bình thường
        Time.timeScale = 1.0f;

        isStopping = false;
    }
}