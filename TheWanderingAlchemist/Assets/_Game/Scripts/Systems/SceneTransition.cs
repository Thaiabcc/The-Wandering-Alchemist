using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Kéo cái Panel đen vào đây")]
    public CanvasGroup fadeCanvasGroup;

    [Header("Tốc độ chuyển cảnh")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad đã có ở Canvas cha rồi nên ko cần thêm ở đây
        }
        else Destroy(gameObject);
    }

    // --- HÀM GỌI TỪ BÊN NGOÀI ---
    public void SwitchScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    // --- LOGIC CHÍNH ---
    IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. FADE OUT (Màn hình tối dần)
        // Code cũ có thể bro đang để LoadSceneAsync chạy song song hoặc ngay sát đít
        yield return Fade(1);

        // Gọi dọn dẹp rác bộ nhớ trước khi load (Giúp nhẹ máy)
        System.GC.Collect();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // --- ĐOẠN QUAN TRỌNG NHẤT ---

        // 1. Cho phép map mới hiện lên
        operation.allowSceneActivation = true;

        // 2. Chờ map mới khởi động xong (Wait until done)
        while (!operation.isDone)
        {
            yield return null;
        }

        // 3. MẤU CHỐT Ở ĐÂY: "Cho nó khựng trong bóng tối"
        // Lúc này các hàm Start(), Physics đang chạy ầm ầm gây lag
        // Ta cứ giữ màn hình đen thêm 0.5s - 1s để che đi cái sự lag đó
        yield return new WaitForSeconds(0.8f);

        // 4. Sau khi mọi thứ đã êm ru, FPS ổn định lại -> Mới cho sáng dần lên
        yield return Fade(0);
    }

    // Hàm phụ trách việc làm mờ/sáng
    IEnumerator Fade(float targetAlpha)
    {
        fadeCanvasGroup.blocksRaycasts = true; // Chặn click luôn từ đầu cho chắc

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        // --- ĐOẠN QUAN TRỌNG NHẤT ĐÂY ---

        // 1. Chốt đơn giá trị cuối cùng
        fadeCanvasGroup.alpha = targetAlpha;

        // 2. Ép GPU vẽ xong frame này mới được đi tiếp
        // Lệnh này bảo Unity: "Đợi tao vẽ xong cái màn đen này đã rồi mày muốn làm gì thì làm"
        yield return new WaitForEndOfFrame();

        // 3. Chờ thêm 1 frame bình thường nữa cho chắc chắn 100% (Double check)
        yield return null;

        // ---------------------------------

        // Nếu là Fade In (Sáng lại) thì mới cho phép click
        if (targetAlpha == 0) fadeCanvasGroup.blocksRaycasts = false;
    }
}